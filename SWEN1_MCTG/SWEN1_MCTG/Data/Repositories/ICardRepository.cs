using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories;

public interface ICardRepository : IRepository<Card>
{
    Card GetByName(string name);
}