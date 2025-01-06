using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Card GetByName(string name);
    List<Card> GetRandomCards(int count);
}