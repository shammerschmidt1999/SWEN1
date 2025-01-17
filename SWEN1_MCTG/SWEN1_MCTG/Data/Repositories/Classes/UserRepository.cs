using System.Reflection;
using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly string _getByUsernameQuery;
        private readonly string _updateQuery;

        public UserRepository(string connectionString)
            : base(connectionString, "users")
        {
            _getByUsernameQuery = $"SELECT * FROM {TableName} WHERE Username = @Username";
            _updateQuery = $"UPDATE {TableName} SET Username = @Username, Password = @Password, Elo = @Elo, Wins = @Wins, Defeats = @Defeats, Draws = @Draws WHERE Id = @Id";
        }

        protected override User CreateEntity() { return new User(); }

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
                if (property.Name == "UserCards" || property.Name == "UserDeck" || property.Name == "UserCoinPurse")
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

            return $"INSERT INTO {TableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)}) RETURNING Id";
        }

        /// <summary>
        /// Gets a user entity by fetching the users data by searching for the username
        /// </summary>
        /// <param name="username"> Provided username </param>
        /// <returns> Corresponding user entity </returns>
        /// <exception cref="InvalidOperationException"> If user is not found in db </exception>
        public async Task<User> GetByUsernameAsync(string username)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_getByUsernameQuery, connection);
            command.Parameters.AddWithValue("@Username", username);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
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
        public async Task<bool> ExistsAsync(string username)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_getByUsernameQuery, connection);
            command.Parameters.AddWithValue("@Username", username);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        /// <summary>
        /// Updates a user in the db
        /// </summary>
        /// <param name="entity"> The user entity and the data that should be stored in the DB </param>
        public async Task UpdateAsync(User entity)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(_updateQuery, connection);
            command.Parameters.AddWithValue("@Username", entity.Username);
            command.Parameters.AddWithValue("@Password", entity.Password);
            command.Parameters.AddWithValue("@Elo", entity.Elo);
            command.Parameters.AddWithValue("@Id", entity.Id);
            command.Parameters.AddWithValue("@Wins", entity.Wins);
            command.Parameters.AddWithValue("@Defeats", entity.Defeats);
            command.Parameters.AddWithValue("@Draws", entity.Draws);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Validates the given credentials
        /// </summary>
        /// <param name="username"> Given username </param>
        /// <param name="password"> Given password </param>
        /// <returns> User entity with provided credentials </returns>
        public async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            string connectionString = AppSettings.GetConnectionString("DefaultConnection");

            await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand("SELECT id, username, password FROM users WHERE username = @username AND password = @password", connection);
            string hashedPassword = PasswordHelper.HashPassword(password);

            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", hashedPassword);

            await using NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                    );
            }

            return null;
        }
    }
}
