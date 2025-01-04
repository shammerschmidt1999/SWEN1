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
        public void AddStack_AddsStackToDatabase()
        {
            // Arrange
            int testUserId = 1;

            IStackRepository stackRepository = new StackRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);

            Card testMonsterCard = cardRepository.GetByName("TestMonsterCard");
            Card testSpellCard = cardRepository.GetByName("TestSpellCard");

            Stack retrievedStackBefore = stackRepository.GetByUserId(testUserId);
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
            stackRepository.Add(stack);

            // Assert
            Stack retrievedStack = stackRepository.GetByUserId(testUserId);
            Assert.IsNotNull(retrievedStack);
            Assert.AreEqual(stackSizeBefore + 2, retrievedStack.Cards.Count);
        }

        [TestMethod]
        public void CreateCard_CreatesCorrectCardType()
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
            Card card = stackRepository.CreateCard(reader);

            // Assert
            Assert.IsNotNull(card);
            Assert.IsInstanceOfType(card, typeof(MonsterCard));
        }

        [TestMethod]
        public void GetByUserId_ReturnsCorrectStack()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            int testUserId = 1;

            // Act
            Stack stack = stackRepository.GetByUserId(testUserId);

            // Assert
            Assert.IsNotNull(stack);
            Assert.AreEqual(testUserId, stack.UserId);
        }

        [TestMethod]
        public void GetByCardId_ReturnsCorrectStack()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            int testCardId = 1;

            // Act
            Stack stack = stackRepository.GetByCardId(testCardId);

            // Assert
            Assert.IsNotNull(stack);
            Assert.IsTrue(stack.Cards.Any(c => c.Id == testCardId));
        }

        [TestMethod]
        public void SetCardInDeck_SetsCardInDeck()
        {
            // Arrange
            IStackRepository stackRepository = new StackRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);
            int testUserId = 1;

            // Create or retrieve a card instance
            Card testCard = cardRepository.GetByName("TestMonsterCard");

            // Add the card to the user's stack
            Stack stack = new Stack()
            {
                UserId = testUserId,
                Cards = new List<Card> { testCard }
            };
            stackRepository.Add(stack);

            // Act
            stackRepository.SetCardInDeck(true, testCard.Id, testUserId);

            // Assert
            Stack retrievedStack = stackRepository.GetByUserId(testUserId);
            Assert.IsTrue(retrievedStack.Cards.Any(c => c.Id == testCard.Id && c.InDeck));
        }


    }
}
