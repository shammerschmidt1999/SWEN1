using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface IStackRepository : IRepository<Stack>
{
    Task<Card> CreateCardAsync(NpgsqlDataReader reader);
    Task<List<Card>> GetUserDeckAsync(User user);
    Task<Stack> GetByUserIdAsync(int id);
    Task<Stack> GetByCardIdAsync(int id);
    Task SetCardInDeckAsync(bool inDeck, int cardId, int userId);
    Task AddCardsToUserAsync(int userId, List<Card> cards);
}