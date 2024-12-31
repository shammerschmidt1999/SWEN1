using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString, "users")
        {
        }

        public new void Add(User entity)
        {
            // Hash the password before adding the user
            entity.Password = PasswordHelper.HashPassword(entity.Password);
            base.Add(entity);
        }

        public User GetByUsername(string username)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Username = @Username";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"User with Username {username} not found.");
        }
    }
}