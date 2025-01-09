using System.Reflection;
using Newtonsoft.Json;
using Npgsql;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    /// <summary>
    /// Repository for a specific entity type.
    /// </summary>
    /// <typeparam name="T"> The class of the entity </typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly string ConnectionString; // Connection string to the database
        protected readonly string TableName; // Name of the table in the database
        protected readonly string GetAllQuery;
        protected readonly string GetByIdQuery;
        protected readonly string DeleteQuery;

        // Constructor to set the connection string, table name and queries
        protected Repository(string connectionString, string tableName)
        {
            ConnectionString = connectionString;
            TableName = tableName;
            GetAllQuery = $"SELECT * FROM {TableName}";
            GetByIdQuery = $"SELECT * FROM {TableName} WHERE Id = @Id";
            DeleteQuery = $"DELETE FROM {TableName} WHERE Id = @Id";
        }

        /// <summary>
        /// Method to add an entity to the database.
        /// </summary>
        /// <param name="entity"> The entity which has the data to be stored in the DB</param>
        public async Task AddAsync(T entity)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Generate the insert query
            string insertQuery = GenerateInsertQuery(entity);

            // Create a command with the insert query and the connection
            await using NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);
            AddParameters(command, entity); // Add the parameters to the command

            // Execute the command
            PropertyInfo idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, Convert.ToInt32(await command.ExecuteScalarAsync()));
            }
        }

        /// <summary>
        /// Method to get all entities from the database.
        /// </summary>
        /// <returns> A List T of entities </returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(GetAllQuery, connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            return MapReaderToEntities(reader);
        }

        /// <summary>
        /// Method to get an entity by its Id from the database
        /// </summary>
        /// <param name="id"> The id of the entity </param>
        /// <returns> The entity with the corresponding id </returns>
        /// <exception cref="InvalidOperationException"> If no entity with the given id is found </exception>
        public async Task<T> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(GetByIdQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"{typeof(T).Name} with Id {id} not found.");
        }

        /// <summary>
        /// Method to delete an entity from the database by its Id
        /// </summary>
        /// <param name="id"> The id of the entity </param>
        public async Task DeleteAsync(int id)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand(DeleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Method to map a reader to a list of entities
        /// </summary>
        /// <param name="reader"> Reader </param>
        /// <returns> IEnumerable of entities mapped to reader </returns>
        protected IEnumerable<T> MapReaderToEntities(NpgsqlDataReader reader)
        {
            List<T> entities = new List<T>();
            while (reader.Read())
            {
                entities.Add(MapReaderToEntity(reader));
            }

            return entities;
        }

        /// <summary>
        /// Method to map a reader to an entity
        /// </summary>
        /// <param name="reader"> Reader </param>
        /// <returns> An entity mapped to the reader </returns>
        protected abstract T MapReaderToEntity(NpgsqlDataReader reader);

        /// <summary>
        /// Method to generate the INSERT Query for an entity
        /// </summary>
        /// <param name="entity"> The entity </param>
        /// <returns> INSERT Query string </returns>
        protected abstract string GenerateInsertQuery(T entity);

        /// <summary>
        /// Method to add parameters to a command
        /// </summary>
        /// <param name="command"> The command </param>
        /// <param name="entity"> The entity affected </param>
        protected abstract void AddParameters(NpgsqlCommand command, T entity);

        /// <summary>
        /// Creates the entity that uses the Repository
        /// </summary>
        /// <returns> The created entity </returns>
        protected abstract T CreateEntity();
    }
}
