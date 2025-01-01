using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories;

namespace SWEN1_MCTG.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private readonly string ConnectionString = AppSettings.GetConnectionString("DefaultConnection");

        [TestMethod]
        public void TestAddUser()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");

            // Act
            userRepository.Add(user);

            // Assert
            User fetchedUser = userRepository.GetByUsername(user.Username);
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(user.Username, fetchedUser.Username);
        }

        [TestMethod]
        public void TestGetUserById()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");

            userRepository.Add(user);

            // Act
            User fetchedUser = userRepository.GetById(user.Id);

            // Assert
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(user.Username, fetchedUser.Username);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            User user = new User($"TestUser_{Guid.NewGuid()}", "Anotherpassword");

            userRepository.Add(user);

            // Act
            User userToDelete = userRepository.GetByUsername(user.Username);
            userRepository.Delete(userToDelete.Id); // Now delete by ID

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => userRepository.GetById(userToDelete.Id)); // Ensure that the user is deleted
        }

        [TestMethod]
        public void TestCreateUserWithCorrectData()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            string username = $"TestUser_{Guid.NewGuid()}";
            string password = "Anotherpassword";
            string hashedPassword = PasswordHelper.HashPassword(password);
            User user = new User(username, password);

            userRepository.Add(user);

            // Act
            User fetchedUser = userRepository.GetByUsername(username);

            // Assert
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(username, fetchedUser.Username);
            Assert.AreEqual(hashedPassword, fetchedUser.Password);
        }
    }
}