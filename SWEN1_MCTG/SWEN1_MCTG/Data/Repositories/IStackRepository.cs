using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories;

public interface IStackRepository : IRepository<Stack>
{
    Card CreateCard(NpgsqlDataReader reader);
    Stack GetByUserId(int id);
    Stack GetByCardId(int id);
}