using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Data
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        public CardRepository(string connectionString) : base(connectionString, "cards")
        {
        }

        protected override Card CreateEntity()
        {
            // Method should not be called for abstract class
            throw new NotImplementedException();
        }

        public new void Add(Card entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var tableName = "cards";
            var insertQuery = GenerateInsertQuery(entity);

            using var command = new NpgsqlCommand(insertQuery, connection);

            // Add the parameters for the entity
            AddParameters(command, entity);

            // Set the card_type based on whether it's a MonsterCard or SpellCard
            string cardType = entity is MonsterCard ? "MonsterCard" : "SpellCard";
            command.Parameters.AddWithValue("@card_type", cardType);

            // Execute the command and get the generated Id
            entity.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        protected override void AddParameters(NpgsqlCommand command, Card entity)
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
                    command.Parameters.AddWithValue($"@{property.Name}", NpgsqlTypes.NpgsqlDbType.Unknown,
                        value != null ? value.ToString() : DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
                }
            }

            // Handle specific properties for MonsterCard
            if (entity is MonsterCard monsterCard)
            {
                command.Parameters.AddWithValue("@monstertype", NpgsqlTypes.NpgsqlDbType.Unknown,
                    monsterCard.MonsterType.ToString());
            }
        }

        protected override string GenerateInsertQuery(Card entity)
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

            return $"INSERT INTO {_tableName} ({columnNames}) VALUES ({parameterNames}) RETURNING Id";
        }

        protected override Card MapReaderToEntity(NpgsqlDataReader reader)
        {
            var cardType = reader["card_type"].ToString() ?? throw new InvalidOperationException("Card type is null.");
            Card card;

            if (cardType == "MonsterCard")
            {
                card = new MonsterCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    (GlobalEnums.MonsterType)Enum.Parse(typeof(GlobalEnums.MonsterType),
                        reader["monstertype"].ToString() ??
                        throw new InvalidOperationException("Monster type is null.")),
                    Convert.ToDouble(reader["Damage"]),
                    (GlobalEnums.ElementType)Enum.Parse(typeof(GlobalEnums.ElementType),
                        reader["elementtype"].ToString() ??
                        throw new InvalidOperationException("Element type is null."))
                );
            }
            else if (cardType == "SpellCard")
            {
                card = new SpellCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    Convert.ToDouble(reader["Damage"]),
                    (GlobalEnums.ElementType)Enum.Parse(typeof(GlobalEnums.ElementType),
                        reader["elementtype"].ToString() ??
                        throw new InvalidOperationException("Element type is null."))
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

        public Card GetByName(string name)
        {
            var query = $"SELECT * FROM {_tableName} WHERE name = @name";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", name);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Card with Name {name} not found.");
        }
    }
}