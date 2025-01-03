using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;
using System;
using System.Collections.Generic;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Tests
{
    [TestClass]
    public class CoinPurseRepositoryTests
    {
        private readonly string _connectionString = AppSettings.GetConnectionString("DefaultConnection");
        private CoinPurseRepository _coinPurseRepository;

        [TestInitialize]
        public void SetUp()
        {
            _coinPurseRepository = new CoinPurseRepository(_connectionString);
        }

        [TestMethod]
        public void TestAddCoinsToPurse()
        {
            // Arrange
            int userId = 1; // Example user ID
            List<Coin> newCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Silver),
                new Coin(CoinType.Gold)
            };

            // Act
            _coinPurseRepository.AddCoinsToPurse(userId, newCoins);
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
        }

        [TestMethod]
        public void TestRemoveCoinsFromPurse()
        {
            // Arrange
            int userId = 2; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Silver),
                new Coin(CoinType.Silver)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Act
            bool result = _coinPurseRepository.RemoveCoinsFromPurse(userId, CoinType.Silver, 1);
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver));
        }

        [TestMethod]
        public void TestRemoveCoinsFromPurse_InsufficientCoins()
        {
            // Arrange
            int userId = 3; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Silver)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Act
            bool result = _coinPurseRepository.RemoveCoinsFromPurse(userId, CoinType.Gold, 1);

            // Assert
            Assert.IsFalse(result); // Not enough Gold coins to remove
        }

        [TestMethod]
        public void TestConvertCoins()
        {
            // Arrange
            int userId = 4; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Silver)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Act
            bool result = _coinPurseRepository.RemoveCoinsFromPurse(userId, CoinType.Bronze, 2);
            if (result)
            {
                _coinPurseRepository.AddCoinsToPurse(userId, new List<Coin> { new Coin(CoinType.Silver) });
            }
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(2, coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver)); // New coin added
        }

        [TestMethod]
        public void TestGetCoinPurseByUserId()
        {
            // Arrange
            int userId = 5; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Bronze),
                new Coin(CoinType.Gold)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Act
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
        }

        [TestMethod]
        public void TestGetCoinPurseByUserId_NotFound()
        {
            // Arrange
            int userId = 999; // Non-existent user ID

            // Act
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsNull(coinPurse); // CoinPurse should be null for non-existent user
        }

        [TestMethod]
        public void TestUpdateCoinPurse()
        {
            // Arrange
            int userId = 6; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Diamond),
                new Coin(CoinType.Platinum)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Add more coins
            var moreCoins = new List<Coin>
            {
                new Coin(CoinType.Gold),
                new Coin(CoinType.Gold)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, moreCoins);

            // Act
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

            // Assert
            Assert.IsNotNull(coinPurse);
            Assert.AreEqual(2, coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
            Assert.AreEqual(1, coinPurse.Coins.Count(c => c.CoinType == CoinType.Diamond));
        }

        [TestMethod]
        public void TestDeleteCoinPurseByUserId()
        {
            // Arrange
            int userId = 7; // Example user ID
            var initialCoins = new List<Coin>
            {
                new Coin(CoinType.Platinum)
            };
            _coinPurseRepository.AddCoinsToPurse(userId, initialCoins);

            // Act
            _coinPurseRepository.DeleteByUserId(userId);

            // Assert
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);
            Assert.IsNull(coinPurse); // The CoinPurse should be deleted
        }
    }
}
