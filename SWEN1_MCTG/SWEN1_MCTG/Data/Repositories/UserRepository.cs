using Npgsql;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString, "users")
        {
        }

        protected override User CreateEntity()
        {
            return new User();
        }

        protected override User MapReaderToEntity(NpgsqlDataReader reader)
        {
            var entity = CreateEntity();
            foreach (var property in typeof(User).GetProperties())
            {
                // Skip complex types
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    continue;
                }

                var value = reader[property.Name];
                property.SetValue(entity, value == DBNull.Value ? null : value);
            }

            return entity;
        }

        protected override string GenerateInsertQuery(User entity)
        {
            var properties = entity.GetType().GetProperties();
            var columns = properties
                .Where(p => p.Name == "Username" || p.Name == "Password" || p.Name == "Elo")
                .Select(p => p.Name);
            var values = properties
                .Where(p => p.Name == "Username" || p.Name == "Password" || p.Name == "Elo")
                .Select(p => $"@{p.Name}");

            if (!columns.Any() || !values.Any())
            {
                throw new InvalidOperationException("No valid properties found to insert.");
            }

            return $"INSERT INTO {_tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
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