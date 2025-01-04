using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Data.Repositories.Classes;

namespace SWEN1_MCTG.Tests
{
    [TestClass]
    public class PackageServiceTests
    {
        private PackageService _packageService;
        private CoinPurseRepository _coinPurseRepository;
        private CardRepository _cardRepository;
        private StackRepository _stackRepository;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            _connectionString = AppSettings.GetConnectionString("TestConnection");

            // Initialize repositories (assuming they connect to your test database)
            _coinPurseRepository = new CoinPurseRepository(_connectionString);
            _cardRepository = new CardRepository(_connectionString);
            _stackRepository = new StackRepository(_connectionString);
        }

        [TestMethod]
        public void TestPurchaseBasicPackage_WithSufficientCoins()
        {
            // Arrange
            int userId = 1; // TestUserID
            CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);
            int previousCardAmount = _stackRepository.GetByUserId(userId).Cards.Count;

            coinPurse.AddCoins(new List<Coin>
            {
                new Coin(GlobalEnums.CoinType.Bronze),
                new Coin(GlobalEnums.CoinType.Bronze),
                new Coin(GlobalEnums.CoinType.Silver)
            });
            int previousCoinValue = coinPurse.GetCoinsValue();
            _coinPurseRepository.UpdateCoinPurse(coinPurse);

            // Mock card selection strategy for testing
            Func<List<Card>, int, List<Card>> cardSelectionStrategy = (cards, toChoose) => cards.Take(toChoose).ToList();

            _packageService = new PackageService(_stackRepository, _cardRepository, _coinPurseRepository, cardSelectionStrategy);

            // Act
            _packageService.PurchasePackage(userId, GlobalEnums.PackageType.Basic);

            // Assert
            CoinPurse updatedCoinPurse = _coinPurseRepository.GetByUserId(userId);
            Stack userStack = _stackRepository.GetByUserId(userId);

            Assert.AreEqual(previousCoinValue - 5, updatedCoinPurse.GetCoinsValue()); // Basic package costs 5 coins
            Assert.AreEqual(previousCardAmount + 1, userStack.Cards.Count); // 1 card added for a Basic package
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPurchasePackage_InsufficientCoins()
        {
            // Arrange
            int userId = 1; // Replace with your test user ID
            var coinPurse = _coinPurseRepository.GetByUserId(userId);
            coinPurse.AddCoins(new List<Coin>
            {
                new Coin(GlobalEnums.CoinType.Bronze) // Only 1 coin, insufficient for any package
            });
            _coinPurseRepository.UpdateCoinPurse(coinPurse);

            // Mock card selection strategy for testing
            Func<List<Card>, int, List<Card>> cardSelectionStrategy = (cards, toChoose) => cards.Take(toChoose).ToList();

            _packageService = new PackageService(_stackRepository, _cardRepository, _coinPurseRepository, cardSelectionStrategy);

            // Act
            _packageService.PurchasePackage(userId, GlobalEnums.PackageType.Basic);

            // Assert
            // Exception is expected
        }
    }
}
