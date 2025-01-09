using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using SWEN1_MCTG.Classes;

public class TokenRepository : ITokenRepository
{
    private readonly string _connectionString;

    public TokenRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // TODO: Add only one token per user to database
    /// <summary>
    /// Creates a token for the user and stores it in the db
    /// </summary>
    /// <param name="token"> The token string </param>
    /// <param name="userId"> The users Id </param>
    public async Task CreateTokenAsync(string token, int userId)
    {
        await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = @"
            INSERT INTO tokens (token, user_id, created_at) 
            VALUES (@token, @userId, @createdAt)";

        await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("token", token);
        command.Parameters.AddWithValue("userId", userId);
        command.Parameters.AddWithValue("createdAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Authenticates the user by accessing the users token
    /// </summary>
    /// <param name="token"> The token string </param>
    /// <returns> TRUE and the user entity if authenticated; FALSE and null if not </returns>
    public async Task<(bool Success, User? User)> AuthenticateAsync(string token)
    {
        await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT u.id, u.username, u.password 
            FROM tokens t 
            JOIN users u ON t.user_id = u.id 
            WHERE t.token = @token 
            ORDER BY t.created_at DESC 
            LIMIT 1";

        await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("token", token);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
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
