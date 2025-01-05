using System.Reflection;
using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly string _getByUsernameQuery;

        public UserRepository(string connectionString)
            : base(connectionString, "users")
        {
            _getByUsernameQuery = $"SELECT * FROM {_tableName} WHERE Username = @Username";
        }

        protected override User CreateEntity() { return new User(); }

        public new void Add(User entity)
        {
            // Hash the password before adding the user
            entity.Password = PasswordHelper.HashPassword(entity.Password);
            base.Add(entity);
        }

        protected override User MapReaderToEntity(NpgsqlDataReader reader)
        {
            User entity = CreateEntity();
            foreach (PropertyInfo property in typeof(User).GetProperties())
            {
                // Skip complex types
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    continue;
                }

                object value = reader[property.Name];
                property.SetValue(entity, value == DBNull.Value ? null : value);
            }

            return entity;
        }

        protected override void AddParameters(NpgsqlCommand command, User user)
        {
            PropertyInfo[] properties = user.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(user);

                // Only consider specific properties
                if (property.Name != "Username" && property.Name != "Password" && property.Name != "Elo")
                {
                    continue;
                }

                // Handle basic types
                NpgsqlParameter parameter = new NpgsqlParameter(property.Name, value ?? DBNull.Value);
                command.Parameters.Add(parameter);
            }
        }

        protected override string GenerateInsertQuery(User entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            IEnumerable<string> columns = properties
                .Where(p => p.Name == "Username" || p.Name == "Password" || p.Name == "Elo")
                .Select(p => p.Name);
            IEnumerable<string> values = properties
                .Where(p => p.Name == "Username" || p.Name == "Password" || p.Name == "Elo")
                .Select(p => $"@{p.Name}");

            if (!columns.Any() || !values.Any())
            {
                throw new InvalidOperationException("No valid properties found to insert.");
            }

            return $"INSERT INTO {_tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)}) RETURNING Id";
        }

        /// <summary>
        /// Gets a user entity by fetching the users data by searching for the username
        /// </summary>
        /// <param name="username"> Provided username </param>
        /// <returns> Corresponding user entity </returns>
        /// <exception cref="InvalidOperationException"> If user is not found in db </exception>
        public User GetByUsername(string username)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getByUsernameQuery, connection);
            command.Parameters.AddWithValue("@Username", username);

            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"User with Username {username} not found.");
        }

        /// <summary>
        /// Checks if a username exists in the db
        /// </summary>
        /// <param name="username"> Provided username </param>
        /// <returns> True if user exits in db; Else false </returns>
        public bool Exists(string username)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getByUsernameQuery, connection);
            command.Parameters.AddWithValue("@Username", username);

            NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }
    }
}