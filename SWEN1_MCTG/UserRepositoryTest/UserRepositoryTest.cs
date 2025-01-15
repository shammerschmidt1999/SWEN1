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