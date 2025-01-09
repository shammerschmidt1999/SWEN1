using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG;

namespace CardRepositoryTest
{
    [TestClass]
    public class CardRepositoryTests
    {
        private readonly string ConnectionString = AppSettings.GetConnectionString("TestConnection");

        [TestMethod]
        public async Task TestAddCardMonsterCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Dragon, 100, GlobalEnums.ElementType.Fire);

            // Act
            await cardRepository.AddAsync(card);

            // Assert
            Card fetchedCard = await cardRepository.GetByNameAsync(card.Name);
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
            Assert.AreEqual(card.Damage, fetchedCard.Damage);
            Assert.AreEqual(card.ElementType, fetchedCard.ElementType);

            // Cast to MonsterCard to assert MonsterType
            MonsterCard? fetchedMonsterCard = fetchedCard as MonsterCard;
            Assert.IsNotNull(fetchedMonsterCard);
            Assert.AreEqual(((MonsterCard)card).MonsterType, fetchedMonsterCard!.MonsterType);
        }

        [TestMethod]
        public async Task TestAddSpellCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new SpellCard($"TestSpellCard{Guid.NewGuid()}", 50, GlobalEnums.ElementType.Fire);

            // Act
            await cardRepository.AddAsync(card);
            Card fetchedCard = await cardRepository.GetByNameAsync(card.Name);

            // Assert
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
            Assert.AreEqual(card.Damage, fetchedCard.Damage);
            Assert.AreEqual(card.ElementType, fetchedCard.ElementType);
        }

        [TestMethod]
        public async Task TestGetCardById()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Knight, 75, GlobalEnums.ElementType.Normal);
            await cardRepository.AddAsync(card);

            // Act
            Card fetchedCard = await cardRepository.GetByIdAsync(card.Id);

            // Assert
            Assert.IsNotNull(fetchedCard);
            Assert.AreEqual(card.Name, fetchedCard.Name);
        }

        [TestMethod]
        public async Task TestDeleteCard()
        {
            // Arrange
            ICardRepository cardRepository = new CardRepository(ConnectionString);
            Card card = new MonsterCard($"TestMonsterCard_{Guid.NewGuid()}", GlobalEnums.MonsterType.Goblin, 50, GlobalEnums.ElementType.Normal);
            await cardRepository.AddAsync(card); // Create the card first

            // Act
            Card cardToDelete = await cardRepository.GetByNameAsync(card.Name);
            await cardRepository.DeleteAsync(cardToDelete.Id);

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await cardRepository.GetByIdAsync(cardToDelete.Id));
        }
    }
}
