using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Linq;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;

namespace SWEN1_MCTG.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly string _connectionString;

        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Card entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var tableName = "Cards"; // Assumes table name is 'Cards'
            var insertQuery = GenerateInsertQuery(tableName, entity) + " RETURNING Id";

            using var command = new NpgsqlCommand(insertQuery, connection);

            // Add the parameters for the entity
            AddParameters(command, entity);

            // Set the card_type based on whether it's a MonsterCard or SpellCard
            string cardType = entity is MonsterCard ? "MonsterCard" : "SpellCard";
            command.Parameters.AddWithValue("@card_type", cardType);

            // Execute the command and get the generated Id
            entity.Id = Convert.ToInt32(command.ExecuteScalar());
        }



        public IEnumerable<Card> GetAll()
        {
            var tableName = "Cards"; // Assumes table name is 'Cards'
            var query = $"SELECT * FROM {tableName}";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            return MapReaderToEntities(reader);
        }

        public Card GetById(int id)
        {
            var tableName = "Cards"; // Assumes table name is 'Cards'
            var query = $"SELECT * FROM {tableName} WHERE Id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Card with Id {id} not found.");
        }

        public Card GetByName(string name)
        {
            var tableName = "Cards"; // Assumes table name is 'Cards'
            var query = $"SELECT * FROM {tableName} WHERE Name = @Name";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Card with Name {name} not found.");
        }

        public void Update(Card entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var tableName = "Cards"; // Assumes table name is 'Cards'
            var updateQuery = GenerateUpdateQuery(tableName, entity);

            using var command = new NpgsqlCommand(updateQuery, connection);
            AddParameters(command, entity);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var tableName = "Cards"; // Assumes table name is 'Cards'
            var query = $"DELETE FROM {tableName} WHERE Id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        private static string GenerateInsertQuery(string tableName, Card entity)
        {
            var properties = typeof(Card).GetProperties().Where(p => p.Name != "Id");
            var columnNames = string.Join(", ", properties.Select(p => p.Name));
            var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            if (entity is MonsterCard)
            {
                columnNames += ", monstertype, card_type";
                parameterNames += ", @monstertype, @card_type";
            }
            else
            {
                columnNames += ", card_type";
                parameterNames += ", @card_type";
            }

            return $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
        }



        private static string GenerateUpdateQuery(string tableName, Card entity)
        {
            var properties = typeof(Card).GetProperties();
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            return $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
        }

        private static void AddParameters(NpgsqlCommand command, Card entity)
        {
            var properties = typeof(Card).GetProperties();

            // Set the card_type explicitly
            string cardType = entity is MonsterCard ? "MonsterCard" : "SpellCard";
            command.Parameters.AddWithValue("@card_type", NpgsqlTypes.NpgsqlDbType.Unknown, cardType);

            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                if (value is Enum)
                {
                    // Convert enums to their underlying string values for PostgreSQL
                    command.Parameters.AddWithValue($"@{property.Name}", NpgsqlTypes.NpgsqlDbType.Unknown, value != null ? value.ToString() : DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
                }
            }

            // Handle specific properties for MonsterCard
            if (entity is MonsterCard monsterCard)
            {
                command.Parameters.AddWithValue("@monstertype", NpgsqlTypes.NpgsqlDbType.Unknown, monsterCard.MonsterType.ToString());
            }
        }







        private static IEnumerable<Card> MapReaderToEntities(IDataReader reader)
        {
            var cards = new List<Card>();
            while (reader.Read())
            {
                cards.Add(MapReaderToEntity(reader));
            }
            return cards;
        }

        private static Card MapReaderToEntity(IDataReader reader)
        {
            var cardType = reader["card_type"].ToString() ?? throw new InvalidOperationException("Card type is null.");
            Card card;

            if (cardType == "MonsterCard")
            {
                card = new MonsterCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    (GlobalEnums.MonsterType)Enum.Parse(typeof(GlobalEnums.MonsterType), reader["monstertype"].ToString() ?? throw new InvalidOperationException("Monster type is null.")),
                    Convert.ToDouble(reader["Damage"]),
                    (GlobalEnums.ElementType)Enum.Parse(typeof(GlobalEnums.ElementType), reader["elementtype"].ToString() ?? throw new InvalidOperationException("Element type is null."))
                );
            }
            else if (cardType == "SpellCard")
            {
                card = new SpellCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    Convert.ToDouble(reader["Damage"]),
                    (GlobalEnums.ElementType)Enum.Parse(typeof(GlobalEnums.ElementType), reader["elementtype"].ToString() ?? throw new InvalidOperationException("Element type is null."))
                );
            }
            else
            {
                throw new InvalidOperationException($"Unknown card type: {cardType}");
            }

            // Set the Id property
            card.Id = Convert.ToInt32(reader["Id"]);

            return card;
        }


    }
}
