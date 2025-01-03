using System.Reflection;
using Newtonsoft.Json;
using Npgsql;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    /// <summary>
    /// Repository for a specific entity type.
    /// </summary>
    /// <typeparam name="T"> The class of the entity </typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string _connectionString; // Connection string to the database
        protected readonly string _tableName; // Name of the table in the database
        protected readonly string _getAllQuery;
        protected readonly string _getByIdQuery;
        protected readonly string _deleteQuery;

        // Constructor to set the connection string, table name and queries
        protected Repository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _getAllQuery = $"SELECT * FROM {_tableName}";
            _getByIdQuery = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            _deleteQuery = $"DELETE FROM {_tableName} WHERE Id = @Id";
        }

        // Abstract method to create an entity of the specific type
        protected abstract T CreateEntity();

        /// <summary>
        /// Method to add an entity to the database.
        /// </summary>
        /// <param name="entity"> The entity which has the data to be stored in the DB</param>
        public void Add(T entity)
        {
            // Open a connection to the database
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Generate the insert query
            string insertQuery = GenerateInsertQuery(entity);

            // Create a command with the insert query and the connection
            NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);
            AddParameters(command, entity); // Add the parameters to the command

            // Execute the command
            PropertyInfo idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, Convert.ToInt32(command.ExecuteScalar()));
            }
        }

        // Method to get all entities from the database
        public IEnumerable<T> GetAll()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getAllQuery, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            return MapReaderToEntities(reader);
        }

        // Method to get an entity by its Id
        public T GetById(int id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getByIdQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"{typeof(T).Name} with Id {id} not found.");
        }

        // Method to update an entity in the database
        public void Update(T entity)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string updateQuery = GenerateUpdateQuery(entity);

            NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
            AddParameters(command, entity);

            command.ExecuteNonQuery();
        }

        // Method to delete an entity from the database
        public void Delete(int id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        // Abstract for now, will be implemented as general method later
        protected abstract string GenerateInsertQuery(T entity);

        // Method to generate the update query for an entity
        protected string GenerateUpdateQuery(T entity)
        {
            IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(p => p.Name != "Id");
            IEnumerable<string> setStatements = properties.Select(p => $"{p.Name} = @{p.Name}");

            return $"UPDATE {_tableName} SET {string.Join(", ", setStatements)} WHERE Id = @Id";
        }

        // Method to add parameters to a command
        protected virtual void AddParameters(NpgsqlCommand command, T entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(entity);

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

        // Method to map a reader to entities
        protected IEnumerable<T> MapReaderToEntities(NpgsqlDataReader reader)
        {
            List<T> entities = new List<T>();
            while (reader.Read())
            {
                entities.Add(MapReaderToEntity(reader));
            }

            return entities;
        }

        // Abstract method to map a reader to an entity
        protected abstract T MapReaderToEntity(NpgsqlDataReader reader);
    }
}