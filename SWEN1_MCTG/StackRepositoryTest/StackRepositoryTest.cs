using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Interfaces;

namespace StackRepositoryTest
{
    [TestClass]
    public class StackRepositoryTests
    {
        private readonly string connectionString = AppSettings.GetConnectionString("TestConnection");

        [TestMethod]
        public async Task AddStack_AddsStackToDatabase()
        {
            // Arrange
            int testUserId = 1;

            IStackRepository stackRepository = new StackRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);

            Card testMonsterCard = await cardRepository.GetByNameAsync("TestMonsterCard");
            Card testSpellCard = await cardRepository.GetByNameAsync("TestSpellCard");

            Stack retrievedStackBefore = await stackRepository.GetByUserIdAsync(testUserId);
            int stackSizeBefore = retrievedStackBefore.Cards.Count;

            Stack stack = new Stack()
            {
                UserId = 1,
                Cards = new List<Card>
                {
                    testMonsterCard,
                    testSpellCard
                }
            };

            // Act
            await stackRepository.AddAsync(stack);

            // Assert
            Stack retrievedStack = await stackRepository.GetByUserIdAsync(testUserId);
            Assert.IsNotNull(retrievedStack);
            Assert.AreEqual(stackSizeBefore + 2, retrievedStack.Cards.Count);
        }

        [TestMethod]
        public async Task CreateCard_CreatesCorrectCardType()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            string commandText = "SELECT * FROM user_cards WHERE card_id = 1";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);
            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();

            // Act
            Card card = await stackRepository.CreateCardAsync(reader);

            // Assert
            Assert.IsNotNull(card);
            Assert.IsInstanceOfType(card, typeof(MonsterCard));
        }

        [TestMethod]
        public async Task GetByUserId_ReturnsCorrectStack()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            int testUserId = 1;

            // Act
            Stack stack = await stackRepository.GetByUserIdAsync(testUserId);

            // Assert
            Assert.IsNotNull(stack);
            Assert.AreEqual(testUserId, stack.UserId);
        }

        [TestMethod]
        public async Task GetByCardId_ReturnsCorrectStack()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            int testCardId = 1;

            // Act
            Stack stack = await stackRepository.GetByCardIdAsync(testCardId);

            // Assert
            Assert.IsNotNull(stack);
            Assert.IsTrue(stack.Cards.Any(c => c.Id == testCardId));
        }

        [TestMethod]
        public async Task SetCardInDeck_SetsCardInDeck()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);
            int testUserId = 1;

            // Create or retrieve a card instance
            Card testCard = await cardRepository.GetByNameAsync("TestMonsterCard");

            // Add the card to the user's stack
            Stack stack = new Stack()
            {
                UserId = testUserId,
                Cards = new List<Card> { testCard }
            };
            await stackRepository.AddAsync(stack);

            // Act
            await stackRepository.SetCardInDeckAsync(true, testCard.Id, testUserId);

            // Assert
            Stack retrievedStack = await stackRepository.GetByUserIdAsync(testUserId);
            Assert.IsTrue(retrievedStack.Cards.Any(c => c.Id == testCard.Id && c.InDeck));
        }


    }
}
