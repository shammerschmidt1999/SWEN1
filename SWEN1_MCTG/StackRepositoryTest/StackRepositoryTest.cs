using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;
using SWEN1_MCTG.Interfaces;

namespace CardRepositoryTest.Classes
{
    [TestClass]
    public class StackRepositoryTests
    {
        private readonly string connectionString = AppSettings.GetConnectionString("DefaultConnection");

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

    }
}
