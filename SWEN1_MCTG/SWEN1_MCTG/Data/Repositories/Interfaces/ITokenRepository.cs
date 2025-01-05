using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ITokenRepository
{
    void CreateToken(string token, int userId);
    (bool Success, User? User) Authenticate(string token);
}