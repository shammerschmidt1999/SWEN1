using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ITokenRepository
{
    Task CreateTokenAsync(string token, int userId);
    Task<(bool Success, User? User)> AuthenticateAsync(string token);
}