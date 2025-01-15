using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ICoinPurseRepository : IRepository<CoinPurse>
{
    List<Coin> GenerateCoins(CoinType coinType, int count);
    Task<CoinPurse> GetByUserIdAsync(int userId);
    Task DeleteByUserIdAsync(int userId);
    Task UpdateCoinPurseAsync(CoinPurse coinPurse);
    Task AddCoinsToPurseAsync(int userId, IEnumerable<Coin> coins);
    Task<bool> RemoveCoinsFromPurseAsync(int userId, int amount);
    Task RemoveAllCoinsFromPurseAsync(int userId);
}