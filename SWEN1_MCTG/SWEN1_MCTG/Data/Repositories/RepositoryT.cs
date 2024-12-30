using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Reflection;

namespace SWEN1_MCTG.Data.Repositories;

public class RepositoryT<T> : IRepository<T> where T : class, new()
{
    private readonly string _connectionString;
    public RepositoryT(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Add(T entity)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var tableName = typeof(T).Name + "s"; // Assumes table name is pluralized
        var insertQuery = GenerateInsertQuery(tableName, entity);

        using var command = new NpgsqlCommand(insertQuery, connection);
        AddParameters(command, entity);

        command.ExecuteNonQuery();
    }

    public IEnumerable<T> GetAll()
    {
        var tableName = typeof(T).Name + "s"; // Assumes table name is pluralized
        var query = $"SELECT * FROM {tableName}";

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        return MapReaderToEntities(reader);
    }

    public T GetById(int id)
    {
        var tableName = typeof(T).Name + "s"; // Assumes table name is pluralized
        var query = $"SELECT * FROM {tableName} WHERE Id = @Id";

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToEntity(reader);
        }

        return null;
    }

    public void Update(T entity)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        var tableName = typeof(T).Name + "s"; // Assumes table name is pluralized
        var updateQuery = GenerateUpdateQuery(tableName, entity);

        using var command = new NpgsqlCommand(updateQuery, connection);
        AddParameters(command, entity);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        var tableName = typeof(T).Name + "s"; // Assumes table name is pluralized
        var query = $"DELETE FROM {tableName} WHERE Id = @Id";

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    private static string GenerateInsertQuery(string tableName, T entity)
    {
        var properties = typeof(T).GetProperties();
        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        return $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
    }

    private static string GenerateUpdateQuery(string tableName, T entity)
    {
        var properties = typeof(T).GetProperties();
        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

        return $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
    }

    private static void AddParameters(NpgsqlCommand command, T entity)
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(entity) ?? DBNull.Value;
            command.Parameters.AddWithValue($"@{property.Name}", value);
        }
    }

    private static IEnumerable<T> MapReaderToEntities(IDataReader reader)
    {
        var entities = new List<T>();
        while (reader.Read())
        {
            entities.Add(MapReaderToEntity(reader));
        }
        return entities;
    }

    private static T MapReaderToEntity(IDataReader reader)
    {
        var entity = new T();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (reader[property.Name] != DBNull.Value)
            {
                property.SetValue(entity, reader[property.Name]);
            }
        }

        return entity;
    }
}