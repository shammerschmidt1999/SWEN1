using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface IStackRepository : IRepository<Stack>
{
    Card CreateCard(NpgsqlDataReader reader);
    Stack GetByUserId(int id);
    Stack GetByCardId(int id);
    void SetCardInDeck(bool inDeck, int cardId, int userId);
    void AddCardsToUser(int userId, List<Card> cards);
}