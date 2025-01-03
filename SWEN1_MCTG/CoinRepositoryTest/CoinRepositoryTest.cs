using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;

[TestClass]
public class CoinRepositoryTests
{
    private readonly string ConnectionString = AppSettings.GetConnectionString("DefaultConnection");

    [TestMethod]
    public void TestAddCoin()
    {
        // Arrange
        CoinRepository coinRepository = new CoinRepository(ConnectionString);
        Coin coin = new Coin(GlobalEnums.CoinType.Gold)
        {
            CoinPurseId = 1,
            Quantity = 10
        };

        // Act
        coinRepository.Add(coin);

        // Assert
        var fetchedCoin = coinRepository.GetById(coin.Id);
        Assert.IsNotNull(fetchedCoin);
        Assert.AreEqual(coin.CoinType, fetchedCoin.CoinType);
        Assert.AreEqual(coin.Quantity, fetchedCoin.Quantity);
    }

    [TestMethod]
    public void TestGetCoinsByCoinPurseId()
    {
        // Arrange
        CoinRepository coinRepository = new CoinRepository(ConnectionString);
        Coin coin1 = new Coin(GlobalEnums.CoinType.Gold) { CoinPurseId = 1, Quantity = 5 };
        Coin coin2 = new Coin(GlobalEnums.CoinType.Silver) { CoinPurseId = 1, Quantity = 3 };
        coinRepository.Add(coin1);
        coinRepository.Add(coin2);

        // Act
        var coins = coinRepository.GetByCoinPurseId(1);

        // Assert
        Assert.IsNotNull(coins);
        Assert.IsTrue(coins.Any(c => c.CoinType == GlobalEnums.CoinType.Gold));
        Assert.IsTrue(coins.Any(c => c.CoinType == GlobalEnums.CoinType.Silver));
        Assert.AreEqual(2, coins.Count());
    }

    [TestMethod]
    public void TestRemoveCoins()
    {
        // Arrange
        CoinRepository coinRepository = new CoinRepository(ConnectionString);
        Coin coin = new Coin(GlobalEnums.CoinType.Bronze) { CoinPurseId = 1, Quantity = 5 };
        coinRepository.Add(coin);

        // Act
        coinRepository.Delete(coin.Id);

        // Assert
        Assert.ThrowsException<InvalidOperationException>(() => coinRepository.GetById(coin.Id));
    }


}