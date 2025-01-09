using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

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
            _coinPurseRepository = new CoinPurseRepository(_connectionString);
            _cardRepository = new CardRepository(_connectionString);
            _stackRepository = new StackRepository(_connectionString);
        }

        [TestMethod]
        public async Task TestPurchaseBasicPackage_WithSufficientCoins()
        {
            // Arrange
            int userId = 1; // TestUserID
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            Stack previousStack = await _stackRepository.GetByUserIdAsync(userId);
            int previousCardAmount = previousStack.Cards.Count;

            coinPurse.AddCoins(new List<Coin>
            {
                new Coin(GlobalEnums.CoinType.Bronze),
                new Coin(GlobalEnums.CoinType.Bronze),
                new Coin(GlobalEnums.CoinType.Silver)
            });

            int previousCoinValue = coinPurse.GetCoinsValue();
            await _coinPurseRepository.UpdateCoinPurseAsync(coinPurse);

            // Mock card selection strategy for testing
            Func<List<Card>, int, List<Card>> cardSelectionStrategy = (cards, toChoose) => cards.Take(toChoose).ToList();

            _packageService = new PackageService(_stackRepository, _cardRepository, _coinPurseRepository, cardSelectionStrategy);

            // Act
            await _packageService.PurchasePackageAsync(userId, GlobalEnums.PackageType.Basic);

            // Assert
            CoinPurse updatedCoinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            Stack userStack = await _stackRepository.GetByUserIdAsync(userId);

            Assert.AreEqual(previousCoinValue - 5, updatedCoinPurse.GetCoinsValue()); // Basic package costs 5 coins
            Assert.AreEqual(previousCardAmount + 1, userStack.Cards.Count); // 1 card added for a Basic package
        }

        [TestMethod]
        public async Task TestPurchaseLegendaryPackage_WithSufficientCoins()
        {
            // Arrange
            int userId = 1; // TestUserID
            await _coinPurseRepository.GetByUserIdAsync(userId); // Clear all coins
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            Stack previousStack = await _stackRepository.GetByUserIdAsync(userId);
            int previousCardAmount = previousStack.Cards.Count;

            coinPurse.AddCoins(new List<Coin>
            {
                new Coin(GlobalEnums.CoinType.Platinum),
                new Coin(GlobalEnums.CoinType.Gold),
                new Coin(GlobalEnums.CoinType.Gold),
                new Coin(GlobalEnums.CoinType.Bronze)
            });

            coinPurse.ConvertCoins(GlobalEnums.CoinType.Gold, GlobalEnums.CoinType.Platinum); // 2 Gold -> 1 Platinum
            coinPurse.ConvertCoins(GlobalEnums.CoinType.Platinum, GlobalEnums.CoinType.Diamond); // 2 Platinum -> 1 Diamond

            await _coinPurseRepository.UpdateCoinPurseAsync(coinPurse);

            // Mock card selection strategy for testing
            Func<List<Card>, int, List<Card>> cardSelectionStrategy = (cards, toChoose) => cards.Take(toChoose).ToList();

            // Act
            _packageService = new PackageService(_stackRepository, _cardRepository, _coinPurseRepository, cardSelectionStrategy);

            await _packageService.PurchasePackageAsync(userId, GlobalEnums.PackageType.Legendary);
            CoinPurse updatedCoinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            Stack userStack = await _stackRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.AreEqual(1, updatedCoinPurse.GetCoinsValue()); // All coins used except 1 Bronze
            Assert.AreEqual(previousCardAmount + 2, userStack.Cards.Count); // 2 cards added for a Legendary package
        }

        [TestMethod]
        public async Task TestPurchasePackage_InsufficientCoins()
        {
            // Arrange
            int userId = 1;
            await _coinPurseRepository.GetByUserIdAsync(userId); // Clear all coins
            CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);
            coinPurse.AddCoins(new List<Coin>
            {
                new Coin(GlobalEnums.CoinType.Bronze) // Only 1 coin, insufficient for any package
            });
            await _coinPurseRepository.UpdateCoinPurseAsync(coinPurse);

            // Mock card selection strategy for testing
            Func<List<Card>, int, List<Card>> cardSelectionStrategy = (cards, toChoose) => cards.Take(toChoose).ToList();

            _packageService = new PackageService(_stackRepository, _cardRepository, _coinPurseRepository, cardSelectionStrategy);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _packageService.PurchasePackageAsync(userId, GlobalEnums.PackageType.Basic));
        }
    }
}
