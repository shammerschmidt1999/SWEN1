using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace UserRepositoryTest
{
    [TestClass]
    public class UserRepositoryTests
    {
        private readonly string ConnectionString = AppSettings.GetConnectionString("TestConnection");

        [TestMethod]
        public async Task TestAddUser()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");

            // Act
            await userRepository.AddAsync(user);

            // Assert
            User fetchedUser = await userRepository.GetByUsernameAsync(user.Username);
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(user.Username, fetchedUser.Username);
        }

        [TestMethod]
        public async Task TestUserStatsAfterVictory()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");
            int oldElo = user.Elo;
            int oldWins = user.Wins;

            // Act
            await userRepository.AddAsync(user);
            User createdUser = await userRepository.GetByUsernameAsync(user.Username);

            createdUser.ApplyBattleResult(GlobalEnums.RoundResults.Victory);
            await userRepository.UpdateAsync(createdUser);

            User updatedUser = await userRepository.GetByUsernameAsync(user.Username);

            // Assert
            Assert.AreEqual(oldElo + 5, updatedUser.Elo);
            Assert.AreEqual(oldWins + 1, updatedUser.Wins);
        }

        [TestMethod]
        public async Task TestUserStatsAfterDraw()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");
            int oldElo = user.Elo;
            int oldDraws = user.Draws;

            // Act
            await userRepository.AddAsync(user);
            User createdUser = await userRepository.GetByUsernameAsync(user.Username);

            createdUser.ApplyBattleResult(GlobalEnums.RoundResults.Draw);
            await userRepository.UpdateAsync(createdUser);

            User updatedUser = await userRepository.GetByUsernameAsync(user.Username);

            // Assert
            Assert.AreEqual(oldElo, updatedUser.Elo);
            Assert.AreEqual(oldDraws + 1, updatedUser.Draws);
        }

        [TestMethod]
        public async Task TestUserStatsAfterDefeat()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");
            int oldElo = user.Elo;
            int oldDefeats = user.Defeats;

            // Act
            await userRepository.AddAsync(user);
            User createdUser = await userRepository.GetByUsernameAsync(user.Username);

            createdUser.ApplyBattleResult(GlobalEnums.RoundResults.Defeat);
            await userRepository.UpdateAsync(createdUser);

            User updatedUser = await userRepository.GetByUsernameAsync(user.Username);

            // Assert
            Assert.AreEqual(oldElo - 3, updatedUser.Elo);
            Assert.AreEqual(oldDefeats + 1, updatedUser.Defeats);
        }

        [TestMethod]
        public async Task TestCreateUserWithCorrectData()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            string username = $"TestUser_{Guid.NewGuid()}";
            string password = "Anotherpassword";
            string hashedPassword = PasswordHelper.HashPassword(password);
            User user = new User(username, password);

            await userRepository.AddAsync(user);

            // Act
            User fetchedUser = await userRepository.GetByUsernameAsync(username);

            // Assert
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(username, fetchedUser.Username);
            Assert.AreEqual(hashedPassword, fetchedUser.Password);
        }
    }
}