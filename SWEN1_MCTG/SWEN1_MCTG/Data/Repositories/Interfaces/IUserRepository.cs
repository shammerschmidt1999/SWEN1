using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(string username);
    Task UpdateAsync(User entity);
    Task<User?> ValidateCredentialsAsync(string username, string password);
}