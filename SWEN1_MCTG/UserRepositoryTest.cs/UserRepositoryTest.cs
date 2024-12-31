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
            var user = new User("Samuel", "Anotherpassword");

            // Act
            userRepository.Add(user);

            // Assert
            var fetchedUser = userRepository.GetByUsername(user.Username);
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(user.Username, fetchedUser.Username);
        }

        [TestMethod]
        public void TestGetUserById()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            var user = new User("Marius", "Password");
            userRepository.Add(user);

            // Act
            var fetchedUser = userRepository.GetById(user.Id);

            // Assert
            Assert.IsNotNull(fetchedUser);
            Assert.AreEqual(user.Username, fetchedUser.Username);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            // Arrange
            IUserRepository userRepository = new UserRepository(ConnectionString);
            var user = new User("Bena", "WordPass");
            userRepository.Add(user);

            // Act
            var userToDelete = userRepository.GetByUsername(user.Username);
            userRepository.Delete(userToDelete.Id); // Now delete by ID

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => userRepository.GetById(userToDelete.Id)); // Ensure that the user is deleted
        }

    }
}