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
    }
}
