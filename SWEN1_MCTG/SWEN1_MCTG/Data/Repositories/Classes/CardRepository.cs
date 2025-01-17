using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        private readonly string _getByNameQuery;

        public CardRepository(string connectionString)
            : base(connectionString, "cards")
        {
            _getByNameQuery = $"SELECT * FROM {TableName} WHERE name = @name";
        }

        protected override Card CreateEntity()
        {
            // Method should not be called for abstract class
            throw new NotImplementedException();
        }

        public new async Task AddAsync(Card entity)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string insertQuery = GenerateInsertQuery(entity);

            await using NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);

            // Add the parameters for the entity
            AddParameters(command, entity);

            // Set the card_type based on whether it's a MonsterCard or SpellCard
            string cardType = entity is MonsterCard ? "MonsterCard" : "SpellCard";
            command.Parameters.AddWithValue("@card_type", cardType);

            // Execute the command and get the generated Id
            entity.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        protected override void AddParameters(NpgsqlCommand command, Card entity)
        {
            PropertyInfo[] properties = typeof(Card).GetProperties();

            // Set the card_type explicitly
            string cardType = entity is MonsterCard ? "MonsterCard" : "SpellCard";
            command.Parameters.AddWithValue("@card_type", NpgsqlTypes.NpgsqlDbType.Unknown, cardType);

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "InDeck" || property.Name == "InstanceId") continue; // Skip the InDeck and InstanceID property

                object? value = property.GetValue(entity);
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
            IEnumerable<PropertyInfo> properties = typeof(Card).GetProperties().Where(p => p.Name != "Id" && p.Name != "InDeck" && p.Name != "InstanceId");
            string columnNames = string.Join(", ", properties.Select(p => p.Name));
            string parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

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

            return $"INSERT INTO {TableName} ({columnNames}) VALUES ({parameterNames}) RETURNING Id";
        }

        protected override Card MapReaderToEntity(NpgsqlDataReader reader)
        {
            string cardType = reader["card_type"].ToString() ?? throw new InvalidOperationException("Card type is null.");
            Card card;

            if (cardType == "MonsterCard")
            {
                card = new MonsterCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    (MonsterType)Enum.Parse(typeof(MonsterType),
                        reader["monstertype"].ToString() ?? throw new InvalidOperationException("Monster type is null.")),
                    Convert.ToDouble(reader["Damage"]),
                    (ElementType)Enum.Parse(typeof(ElementType),
                        reader["elementtype"].ToString() ?? throw new InvalidOperationException("Element type is null."))
                );
            }
            else if (cardType == "SpellCard")
            {
                card = new SpellCard(
                    reader["Name"].ToString() ?? throw new InvalidOperationException("Name is null."),
                    Convert.ToDouble(reader["Damage"]),
                    (ElementType)Enum.Parse(typeof(ElementType),
                        reader["elementtype"].ToString() ?? throw new InvalidOperationException("Element type is null."))
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

        public async Task<List<Card>> GetRandomCardsAsync(int count)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string query = $"SELECT * FROM cards ORDER BY RANDOM() LIMIT {count}";
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);

            List<Card> cards = new List<Card>();
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cards.Add(MapReaderToEntity(reader));
            }

            return cards;
        }
    }
}
