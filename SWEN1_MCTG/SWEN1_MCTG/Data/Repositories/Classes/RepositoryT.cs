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

        /// <summary>
        /// Method to get all entities from the database.
        /// </summary>
        /// <returns> A List T of entities </returns>
        public IEnumerable<T> GetAll()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getAllQuery, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            return MapReaderToEntities(reader);
        }

        /// <summary>
        /// Method to get an entity by its Id from the database
        /// </summary>
        /// <param name="id"> The id of the entity </param>
        /// <returns> The entity with the corresponding id </returns>
        /// <exception cref="InvalidOperationException"> If no entity with the given id is found </exception>
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

        /// <summary>
        /// Method to delete an entity from the database by its Id
        /// </summary>
        /// <param name="id"> The id of the entity </param>
        public void Delete(int id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_deleteQuery, connection);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
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
    }
}