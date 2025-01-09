using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;
using System;
using System.Collections.Generic;
using static SWEN1_MCTG.GlobalEnums;

namespace CoinPurseRepositoryTest
{
    [TestClass]
    public class CoinPurseRepositoryTests
    {
        private readonly string _connectionString = AppSettings.GetConnectionString("TestConnection");
        private CoinPurseRepository _coinPurseRepository;

        [TestInitialize]
        public void SetUp()
        {
            _coinPurseRepository = new CoinPurseRepository(_connectionString);
        }

        [TestMethod]
        public async Task TestAddCoinsToPurse()
        {
            // Arrange
            int userId = 1;
            List<Coin> newCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Silver),
                new Coin(CoinType.Gold)
            };

            // Act
            await _coinPurseRepository.AddCoinsToPurseAsync(userId, newCoins);
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
        }

        [TestMethod]
        public async Task TestGetCoinPurseByUserId()
        {
            // Arrange
            int userId = 5; // Example user ID
            List<Coin> initialCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Gold)
            };
            await _coinPurseRepository.AddCoinsToPurseAsync(userId, initialCoins);

            // Act
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
        }

        [TestMethod]
        public async Task TestGetCoinPurseByUserId_NotFound()
        {
            // Arrange
            int userId = 999; // Non-existent user ID

            // Act
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNull(coinPurse); // CoinPurse should be null for non-existent user
        }

        [TestMethod]
        public async Task TestUpdateCoinPurse()
        {
            // Arrange
            int userId = 6;
            List<Coin> initialCoins = new List<Coin>
            {
                new Coin(CoinType.Diamond),
                new Coin(CoinType.Platinum)
            };
            await _coinPurseRepository.AddCoinsToPurseAsync(userId, initialCoins);

            // Add more coins
            List<Coin> moreCoins = new List<Coin>
            {
                new Coin(CoinType.Gold),
                new Coin(CoinType.Gold)
            };
            await _coinPurseRepository.AddCoinsToPurseAsync(userId, moreCoins);

            // Act
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(2, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Diamond));
        }

        [TestMethod]
        public async Task TestDeleteCoinPurseByUserId()
        {
            // Arrange
            int userId = 7;
            List<Coin> initialCoins = new List<Coin>
            {
                new Coin(CoinType.Platinum)
            };
            await _coinPurseRepository.AddCoinsToPurseAsync(userId, initialCoins);

            // Act
            await _coinPurseRepository.GetByUserIdAsync(userId);

            // Assert
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            Assert.IsNull(coinPurse); // The CoinPurse should be deleted
        }
    }
}
