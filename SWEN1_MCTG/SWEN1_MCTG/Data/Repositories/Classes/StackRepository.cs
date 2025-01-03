using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class StackRepository : Repository<Stack>, IStackRepository
    {
        private readonly CardRepository _cardRepository;
        private readonly string _nToMUserQueryString;
        private readonly string _nToMCardQueryString;
        private readonly string _nToMInsertQueryString;

        public StackRepository(string connectionString)
            : base(connectionString, "user_cards")
        {
            _cardRepository = new CardRepository(connectionString);
            _nToMUserQueryString = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {_tableName} uc
                WHERE uc.user_id = @UserId";

            _nToMCardQueryString = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {_tableName} uc
                WHERE uc.card_id = @CardId";

            _nToMInsertQueryString = $"INSERT INTO {_tableName} (user_id, card_id, in_deck, card_type, instance_id) " +
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
            {
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                Cards = new List<Card>()
            };

            do
            {
                Card card = CreateCard(reader);
                stack.Cards.Add(card);
            } while (reader.Read());

            return stack;
        }

        public Card CreateCard(NpgsqlDataReader reader)
        {
            int cardId = reader.GetInt32(reader.GetOrdinal("card_id"));
            bool inDeck = reader.GetBoolean(reader.GetOrdinal("in_deck"));
            string cardType = reader.GetString(reader.GetOrdinal("card_type"));

            // Fetch card details using CardRepository
            Card card = _cardRepository.GetById(cardId);
            card.SetInDeck(inDeck);

            return card;
        }

        public Stack GetByUserId(int userId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_nToMUserQueryString, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Stack for user with Id {userId} not found.");
        }

        public Stack GetByCardId(int cardId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_nToMCardQueryString, connection);
            command.Parameters.AddWithValue("@CardId", cardId);

            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Stack for card with Id {cardId} not found.");
        }

        public void SetCardInDeck(bool inDeck, int cardId, int userId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Fetch the specific card instance from the database
            string selectQuery = $@"
            SELECT instance_id
            FROM {_tableName}
            WHERE card_id = @CardId AND user_id = @UserId
            LIMIT 1";

            NpgsqlCommand selectCommand = new NpgsqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@CardId", cardId);
            selectCommand.Parameters.AddWithValue("@UserId", userId);

            Guid instanceId;
            using (NpgsqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (reader.Read())
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
            UPDATE {_tableName}
            SET in_deck = @InDeck
            WHERE card_id = @CardId AND user_id = @UserId AND instance_id = @InstanceId";

            NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@InDeck", inDeck);
            updateCommand.Parameters.AddWithValue("@CardId", cardId);
            updateCommand.Parameters.AddWithValue("@UserId", userId);
            updateCommand.Parameters.AddWithValue("@InstanceId", instanceId);

            int rowsAffected = updateCommand.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Card instance with Id {cardId} and InstanceId {instanceId} not found for user with Id {userId}");
            }
        }


    }
}
