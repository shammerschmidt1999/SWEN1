using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ICoinPurseRepository : IRepository<CoinPurse>
{
    List<Coin> GenerateCoins(CoinType coinType, int count);
    Task<CoinPurse> GetByUserIdAsync(int userId);
    Task UpdateCoinPurseAsync(CoinPurse coinPurse);
}