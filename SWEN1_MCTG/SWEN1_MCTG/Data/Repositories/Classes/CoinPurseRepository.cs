using Npgsql;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Data.Repositories.Classes
{
    public class CoinPurseRepository : Repository<CoinPurse>, ICoinPurseRepository
    {
        private readonly string _getCoinPurseByIdQuery = @"SELECT * FROM coin_purses WHERE user_id = @UserId";
        private readonly string _deleteCoinPurseQuery = @"DELETE FROM coin_purses WHERE user_id = @UserId";
        private readonly string _updateCoinPurseQuery = @"UPDATE coin_purses SET Bronze = 
            @Bronze, Silver = @Silver, Gold = @Gold, Platinum = @Platinum, Diamond = @Diamond WHERE user_id = @UserId";
        private readonly string _insertCoinPurseQuery = @"INSERT INTO coin_purses (user_id, bronze, silver, gold, platinum, diamond) 
            VALUES (@UserId, @Bronze, @Silver, @Gold, @Platinum, @Diamond) 
            ON CONFLICT (user_id) DO UPDATE SET bronze = @Bronze, silver = @Silver, gold = @Gold, platinum = @Platinum, diamond = @Diamond";

        public CoinPurseRepository(string connectionString)
            : base(connectionString, "coin_purses")
        {
        }

        public List<Coin> GenerateCoins(CoinType coinType, int count)
        {
            List<Coin> coins = new List<Coin>();
            for (int i = 0; i < count; i++)
            {
                coins.Add(new Coin(coinType));
            }
            return coins;
        }

        public async Task<CoinPurse> GetByUserIdAsync(int userId)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string query = $"SELECT * FROM {TableName} WHERE user_id = @userId";
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToEntity(reader);
            }

            throw new InvalidOperationException($"CoinPurse with UserId {userId} not found.");
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string query = $"DELETE FROM {TableName} WHERE user_id = @userId";
            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task AddCoinPurseAsync(CoinPurse coinPurse)
        {
            await AddAsync(coinPurse);
        }

        public async Task UpdateCoinPurseAsync(CoinPurse coinPurse)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string updateQuery = GenerateUpdateQuery(coinPurse);

            await using NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
            AddParameters(command, coinPurse);

            await command.ExecuteNonQueryAsync();
        }

        public async Task AddCoinsToPurseAsync(int userId, IEnumerable<Coin> coins)
        {
            CoinPurse coinPurse = await GetByUserIdAsync(userId);
            coinPurse.AddCoins(coins);
            await UpdateCoinPurseAsync(coinPurse);
        }

        public async Task<bool> RemoveCoinsFromPurseAsync(int userId, int amount)
        {
            CoinPurse coinPurse = await GetByUserIdAsync(userId);
            bool result = coinPurse.ExtractCoins(amount) != null;
            if (result)
            {
                await UpdateCoinPurseAsync(coinPurse);
            }
            return result;
        }

        public async Task RemoveAllCoinsFromPurseAsync(int userId)
        {
            CoinPurse coinPurse = await GetByUserIdAsync(userId);
            coinPurse.Coins.Clear();
            await UpdateCoinPurseAsync(coinPurse);
        }

        protected override CoinPurse CreateEntity()
        {
            return new CoinPurse();
        }

        protected override void AddParameters(NpgsqlCommand command, CoinPurse entity)
        {
            command.Parameters.AddWithValue("@user_id", entity.UserId);
            command.Parameters.AddWithValue("@coins", entity.Coins);
        }

        protected override string GenerateInsertQuery(CoinPurse entity)
        {
            return $"INSERT INTO {TableName} (user_id, coins) VALUES (@user_id, @coins) RETURNING id";
        }

        protected override CoinPurse MapReaderToEntity(NpgsqlDataReader reader)
        {
            CoinPurse coinPurse = new CoinPurse();
            coinPurse.UserId = reader.GetInt32(reader.GetOrdinal("user_id"));

            // Add coins based on the stored values in the database
            int bronzeCount = reader.GetInt32(reader.GetOrdinal("bronze"));
            int silverCount = reader.GetInt32(reader.GetOrdinal("silver"));
            int goldCount = reader.GetInt32(reader.GetOrdinal("gold"));
            int platinumCount = reader.GetInt32(reader.GetOrdinal("platinum"));
            int diamondCount = reader.GetInt32(reader.GetOrdinal("diamond"));

            coinPurse.AddCoins(GenerateCoins(GlobalEnums.CoinType.Bronze, bronzeCount));
            coinPurse.AddCoins(GenerateCoins(GlobalEnums.CoinType.Silver, silverCount));
            coinPurse.AddCoins(GenerateCoins(GlobalEnums.CoinType.Gold, goldCount));
            coinPurse.AddCoins(GenerateCoins(GlobalEnums.CoinType.Platinum, platinumCount));
            coinPurse.AddCoins(GenerateCoins(GlobalEnums.CoinType.Diamond, diamondCount));

            return coinPurse;
        }

        private string GenerateUpdateQuery(CoinPurse entity)
        {
            return $"UPDATE {TableName} SET coins = @coins WHERE user_id = @user_id";
        }
    }
}
