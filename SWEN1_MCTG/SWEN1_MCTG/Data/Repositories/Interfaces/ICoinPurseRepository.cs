using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ICoinPurseRepository : IRepository<CoinPurse>
{
    List<Coin> GenerateCoins(CoinType coinType, int count);
    CoinPurse GetByUserId(int userId);
    void DeleteByUserId(int userId);
    void AddCoinPurse(CoinPurse coinPurse);
    void UpdateCoinPurse(CoinPurse coinPurse);
    void AddCoinsToPurse(int userId, IEnumerable<Coin> coins);
    public bool RemoveCoinsFromPurse(int userId, int amount);
    void RemoveAllCoinsFromPurse(int userId);
}