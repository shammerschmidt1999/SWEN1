using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Data.Repositories;

namespace SWEN1_MCTG.Tests
{
    [TestClass]
    public class CardRepositoryTests
    {
        private readonly string ConnectionString = AppSettings.GetConnectionString("DefaultConnection");

        [TestMethod]
        public void TestAddCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            var card = new MonsterCard("Dragon", GlobalEnums.MonsterType.Dragon, 100, GlobalEnums.ElementType.Fire);

            // Act
            cardRepository.Add(card);

            // Assert
            var fetchedCard = cardRepository.GetByName(card.Name);
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
        }

        [TestMethod]
        public void TestGetCardById()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            var card = new MonsterCard("Knight", GlobalEnums.MonsterType.Knight, 75, GlobalEnums.ElementType.Normal);
            cardRepository.Add(card); // Create the card first

            // Act
            var fetchedCard = cardRepository.GetById(card.Id);

            // Assert
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
        }

        [TestMethod]
        public void TestDeleteCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            var card = new MonsterCard("DeletedGoblin", GlobalEnums.MonsterType.Goblin, 50, GlobalEnums.ElementType.Normal);
            cardRepository.Add(card); // Create the card first

            // Act
            var cardToDelete = cardRepository.GetByName(card.Name); // Fetch by name to get the card details
            cardRepository.Delete(cardToDelete.Id); // Now delete by ID

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => cardRepository.GetById(cardToDelete.Id)); // Ensure that the card is deleted
        }

    }
}