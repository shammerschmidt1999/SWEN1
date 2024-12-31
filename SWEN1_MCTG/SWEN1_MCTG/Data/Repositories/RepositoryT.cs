using Newtonsoft.Json;
using Npgsql;

namespace SWEN1_MCTG.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;

        protected Repository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public void Add(T entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var insertQuery = GenerateInsertQuery(entity) + " RETURNING Id";

            using var command = new NpgsqlCommand(insertQuery, connection);
            AddParameters(command, entity);

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, Convert.ToInt32(command.ExecuteScalar()));
            }
        }

        public IEnumerable<T> GetAll()
        {
            var query = $"SELECT * FROM {_tableName}";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            return MapReaderToEntities(reader);
        }

        public T GetById(int id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"{typeof(T).Name} with Id {id} not found.");
        }

        public void Update(T entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var updateQuery = GenerateUpdateQuery(entity);

            using var command = new NpgsqlCommand(updateQuery, connection);
            AddParameters(command, entity);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        protected string GenerateInsertQuery(T entity)
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


        protected string GenerateUpdateQuery(T entity)
        {
            var properties = entity.GetType().GetProperties().Where(p => p.Name != "Id");
            var setStatements = properties.Select(p => $"{p.Name} = @{p.Name}");

            return $"UPDATE {_tableName} SET {string.Join(", ", setStatements)} WHERE Id = @Id";
        }

        private void AddParameters(NpgsqlCommand command, T entity)
        {
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(entity);

                // Only consider specific properties
                if (property.Name != "Username" && property.Name != "Password" && property.Name != "Elo")
                {
                    continue;
                }

                // Handle basic types
                var parameter = new NpgsqlParameter(property.Name, value ?? DBNull.Value);
                command.Parameters.Add(parameter);
            }
        }

        protected IEnumerable<T> MapReaderToEntities(NpgsqlDataReader reader)
        {
            var entities = new List<T>();
            while (reader.Read())
            {
                entities.Add(MapReaderToEntity(reader));
            }

            return entities;
        }

        protected virtual T MapReaderToEntity(NpgsqlDataReader reader)
        {
            var entity = new T();
            foreach (var property in typeof(T).GetProperties())
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
    }
}
