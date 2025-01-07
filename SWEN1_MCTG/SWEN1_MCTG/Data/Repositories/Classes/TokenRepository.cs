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

    /// <summary>
    /// Creates a token for the user and stores it in the db
    /// </summary>
    /// <param name="token"> The token string </param>
    /// <param name="userId"> The users Id </param>
    public void CreateToken(string token, int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        NpgsqlCommand command = new NpgsqlCommand("INSERT INTO tokens (token, user_id) VALUES (@token, @userId)", connection);
        command.Parameters.AddWithValue("token", token);
        command.Parameters.AddWithValue("userId", userId);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Authenticates the user by accessing the users token
    /// </summary>
    /// <param name="token"> The token string </param>
    /// <returns> TRUE and the user entity if authenticated; FALSE and null if not </returns>
    public (bool Success, User? User) Authenticate(string token)
    {
        NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        
        NpgsqlCommand command = new NpgsqlCommand("SELECT u.id, u.username, u.password FROM tokens t JOIN users u ON t.user_id = u.id WHERE t.token = @token", connection);
        command.Parameters.AddWithValue("token", token);

        NpgsqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            User user = new User
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