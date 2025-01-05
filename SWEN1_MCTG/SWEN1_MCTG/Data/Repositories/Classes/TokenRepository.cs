using System;
using System.Data;
using Npgsql;
using SWEN1_MCTG.Classes;

public class TokenRepository : ITokenRepository
{
    private readonly string _connectionString;

    public TokenRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateToken(string token, int userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand("INSERT INTO tokens (token, user_id) VALUES (@token, @userId)", connection);
        command.Parameters.AddWithValue("token", token);
        command.Parameters.AddWithValue("userId", userId);

        command.ExecuteNonQuery();
    }

    public (bool Success, User? User) Authenticate(string token)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var command = new NpgsqlCommand("SELECT u.id, u.username, u.password FROM tokens t JOIN users u ON t.user_id = u.id WHERE t.token = @token", connection);
        command.Parameters.AddWithValue("token", token);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var user = new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
            return (true, user);
        }

        return (false, null);
    }
}