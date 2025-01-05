using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    User GetByUsername(string username);
    bool Exists(string username);
}