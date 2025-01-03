using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Data.Repositories;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG;

namespace CardRepositoryTest
{
    [TestClass]
    public class CardRepositoryTests
    {
        private readonly string ConnectionString = AppSettings.GetConnectionString("DefaultConnection");

        [TestMethod]
        public void TestAddCardMonsterCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Dragon, 100, GlobalEnums.ElementType.Fire);

            // Act
            cardRepository.Add(card);

            // Assert
            Card fetchedCard = cardRepository.GetByName(card.Name);
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
            Assert.AreEqual(card.Damage, fetchedCard.Damage);
            Assert.AreEqual(card.ElementType, fetchedCard.ElementType);

            // Cast to MonsterCard to assert MonsterType
            MonsterCard fetchedMonsterCard = fetchedCard as MonsterCard;
            Assert.IsNotNull(fetchedMonsterCard);
            Assert.AreEqual(((MonsterCard)card).MonsterType, fetchedMonsterCard.MonsterType);
        }

        [TestMethod]
        public void TestAddSpellCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new SpellCard($"TestSpellCard{Guid.NewGuid()}", 50, GlobalEnums.ElementType.Fire);
            cardRepository.Add(card);

            // Act
            Card fetchedCard = cardRepository.GetByName(card.Name);

            // Assert
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
            Assert.AreEqual(card.Damage, fetchedCard.Damage);
            Assert.AreEqual(card.ElementType, fetchedCard.ElementType);
        }

        [TestMethod]
        public void TestGetCardById()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Knight, 75, GlobalEnums.ElementType.Normal);
            cardRepository.Add(card);

            // Act
            Card fetchedCard = cardRepository.GetById(card.Id);

            // Assert
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
        }

        [TestMethod]
        public void TestDeleteCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Goblin, 50, GlobalEnums.ElementType.Normal);
            cardRepository.Add(card); // Create the card first

            // Act
            Card cardToDelete = cardRepository.GetByName(card.Name);
            cardRepository.Delete(cardToDelete.Id);

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => cardRepository.GetById(cardToDelete.Id));
        }

    }
}