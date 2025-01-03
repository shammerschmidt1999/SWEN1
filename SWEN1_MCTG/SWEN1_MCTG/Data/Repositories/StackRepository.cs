using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories
{
    public class StackRepository : Repository<Stack>, IStackRepository
    {
        private readonly CardRepository _cardRepository;

        public StackRepository(string connectionString)
            : base(connectionString, "user_cards")
        {
            _cardRepository = new CardRepository(connectionString);
        }

        protected override Stack CreateEntity()
        {
            return new Stack();
        }

        protected override string GenerateInsertQuery(Stack entity)
        {
            return $"INSERT INTO {_tableName} (user_id, card_id, in_deck, card_type, instance_id) VALUES (@UserId, @CardId, @InDeck, @CardType, @InstanceId)";
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
            card.InDeck = inDeck;

            return card;
        }

        public Stack GetByUserId(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string query = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {_tableName} uc
                WHERE uc.user_id = @UserId";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Stack for user with Id {userId} not found.");
        }

        public Stack GetByCardId(int cardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string query = $@"
                SELECT uc.user_id, uc.card_id, uc.in_deck, uc.card_type
                FROM {_tableName} uc
                WHERE uc.card_id = @CardId";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@CardId", cardId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"Stack for card with Id {cardId} not found.");
        }
    }
}
