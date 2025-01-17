using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class StackRepository : Repository<Stack>, IStackRepository
    {
        private readonly CardRepository _cardRepository;
        private readonly string _nToMUserQueryString;
        private readonly string _nToMCardQueryString;
        private readonly string _nToMInsertQueryString;
        private readonly string _getInDeckQueryString;

        public StackRepository(string connectionString)
            : base(connectionString, "user_cards")
        {
            _cardRepository = new CardRepository(connectionString);
            _nToMUserQueryString = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {TableName} uc
                WHERE uc.user_id = @UserId";

            _getInDeckQueryString =
                $@"SELECT uc.card_id FROM {TableName} uc WHERE 
                uc.user_id = @UserId AND uc.in_deck = TRUE";

            _nToMCardQueryString = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {TableName} uc
                WHERE uc.card_id = @CardId";

            _nToMInsertQueryString = $"INSERT INTO {TableName} (user_id, card_id, in_deck, card_type, instance_id) " +
                                     $"VALUES (@UserId, @CardId, @InDeck, @CardType, @InstanceId)";
        }

        protected override Stack CreateEntity()
        {
            return new Stack();
        }

        protected override string GenerateInsertQuery(Stack entity)
        {
            return _nToMInsertQueryString;
        }

        protected override void AddParameters(NpgsqlCommand command, Stack entity)
        {
            foreach (Card card in entity.Cards)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserId", entity.UserId);
                command.Parameters.AddWithValue("@CardId", card.Id);
                command.Parameters.AddWithValue("@InDeck", card.InDeck);

                string cardType = card is MonsterCard ? "MonsterCard" : "SpellCard";
                command.Parameters.AddWithValue("@CardType", NpgsqlTypes.NpgsqlDbType.Unknown, cardType);
                command.Parameters.AddWithValue("@CardType", cardType);

                command.Parameters.AddWithValue("@InstanceId", Guid.NewGuid());

                command.ExecuteNonQuery();
            }
        }

        protected override Stack MapReaderToEntity(NpgsqlDataReader reader)
        {
            Stack stack = new Stack
                (
                reader.GetInt32(reader.GetOrdinal("user_id")),
                new List<Card>()
                );

            do
            {
                Card card = Task.Run(() => CreateCardAsync(reader)).Result;
                stack.Cards.Add(card);
            } while (reader.Read());

            return stack;
        }

        public async Task<List<Card>> GetUserDeckAsync(User user)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_getInDeckQueryString, connection);
            command.Parameters.AddWithValue("@UserId", user.Id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            List<Card> deck = new List<Card>();
            while (reader.Read()) {
                int cardId = reader.GetInt32(reader.GetOrdinal("card_id"));
                Card card = await _cardRepository.GetByIdAsync(cardId);
                deck.Add(card);
            }

            return deck;
        }

        public async Task<Card> CreateCardAsync(NpgsqlDataReader reader)
        {
            int cardId = reader.GetInt32(reader.GetOrdinal("card_id"));
            bool inDeck = reader.GetBoolean(reader.GetOrdinal("in_deck"));
            string cardType = reader.GetString(reader.GetOrdinal("card_type"));

            // Fetch card details using CardRepository
            Card card = await _cardRepository.GetByIdAsync(cardId);
            card.SetInDeck(inDeck);

            return card;
        }

        public async Task<Stack> GetByUserIdAsync(int userId)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_nToMUserQueryString, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            return null;
        }

        public async Task<Stack> GetByCardIdAsync(int cardId)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_nToMCardQueryString, connection);
            command.Parameters.AddWithValue("@CardId", cardId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Stack for card with Id {cardId} not found.");
        }

        public async Task SetCardInDeckAsync(bool inDeck, int cardId, int userId)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Fetch the specific card instance from the database
            string selectQuery = $@"
            SELECT instance_id
            FROM {TableName}
            WHERE card_id = @CardId AND user_id = @UserId
            LIMIT 1";

            await using NpgsqlCommand selectCommand = new NpgsqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@CardId", cardId);
            selectCommand.Parameters.AddWithValue("@UserId", userId);

            Guid instanceId;
            await using (NpgsqlDataReader reader = await selectCommand.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    instanceId = reader.GetGuid(reader.GetOrdinal("instance_id"));
                }
                else
                {
                    throw new InvalidOperationException($"Card with Id {cardId} not found for user with Id {userId}");
                }
            }

            // Update the in_deck status for the specific card instance
            string updateQuery = $@"
            UPDATE {TableName}
            SET in_deck = @InDeck
            WHERE card_id = @CardId AND user_id = @UserId AND instance_id = @InstanceId";

            await using NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@InDeck", inDeck);
            updateCommand.Parameters.AddWithValue("@CardId", cardId);
            updateCommand.Parameters.AddWithValue("@UserId", userId);
            updateCommand.Parameters.AddWithValue("@InstanceId", instanceId);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Card instance with Id {cardId} and InstanceId {instanceId} not found for user with Id {userId}");
            }
        }

        public async Task AddCardsToUserAsync(int userId, List<Card> cards)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using (NpgsqlTransaction transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    foreach (Card card in cards)
                    {
                        await using (NpgsqlCommand command = new NpgsqlCommand(_nToMInsertQueryString, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@CardId", card.Id);
                            command.Parameters.AddWithValue("@InDeck", card.InDeck);

                            string cardType = card is MonsterCard ? "MonsterCard" : "SpellCard";
                            command.Parameters.AddWithValue("@CardType", NpgsqlTypes.NpgsqlDbType.Unknown, cardType);
                            command.Parameters.AddWithValue("@InstanceId", Guid.NewGuid());

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
