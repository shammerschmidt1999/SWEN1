﻿using Npgsql;
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
        private readonly string _updateCoinPurseQuery = @"UPDATE coin_purses SET Bronze = @Bronze, Silver = @Silver, Gold = @Gold, Platinum = @Platinum, Diamond = @Diamond WHERE user_id = @UserId";
        private readonly string _insertCoinPurseQuery = @"INSERT INTO coin_purses (user_id, bronze, silver, gold, platinum, diamond) VALUES (@UserId, @Bronze, @Silver, @Gold, @Platinum, @Diamond) ON CONFLICT (user_id) DO UPDATE SET bronze = @Bronze, silver = @Silver, gold = @Gold, platinum = @Platinum, diamond = @Diamond";

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

        public async Task UpdateCoinPurseAsync(CoinPurse coinPurse)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string updateQuery = GenerateUpdateQuery(coinPurse);

            await using NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
            AddParameters(command, coinPurse);

            await command.ExecuteNonQueryAsync();
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

        protected override CoinPurse CreateEntity()
        {
            return new CoinPurse();
        }

        protected override void AddParameters(NpgsqlCommand command, CoinPurse coinPurse)
        {
            command.Parameters.AddWithValue("@UserId", coinPurse.UserId);
            command.Parameters.AddWithValue("@Bronze", coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            command.Parameters.AddWithValue("@Silver", coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver));
            command.Parameters.AddWithValue("@Gold", coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
            command.Parameters.AddWithValue("@Platinum", coinPurse.Coins.Count(c => c.CoinType == CoinType.Platinum));
            command.Parameters.AddWithValue("@Diamond", coinPurse.Coins.Count(c => c.CoinType == CoinType.Diamond));
        }

        protected override string GenerateInsertQuery(CoinPurse entity)
        {
            return _insertCoinPurseQuery;
        }

        protected override CoinPurse MapReaderToEntity(NpgsqlDataReader reader)
        {
            CoinPurse coinPurse = new CoinPurse(reader.GetInt32(reader.GetOrdinal("user_id")));

            // Add coins based on the stored values in the database
            int bronzeCount = reader.GetInt32(reader.GetOrdinal("bronze"));
            int silverCount = reader.GetInt32(reader.GetOrdinal("silver"));
            int goldCount = reader.GetInt32(reader.GetOrdinal("gold"));
            int platinumCount = reader.GetInt32(reader.GetOrdinal("platinum"));
            int diamondCount = reader.GetInt32(reader.GetOrdinal("diamond"));

            coinPurse.AddCoins(GenerateCoins(CoinType.Bronze, bronzeCount));
            coinPurse.AddCoins(GenerateCoins(CoinType.Silver, silverCount));
            coinPurse.AddCoins(GenerateCoins(CoinType.Gold, goldCount));
            coinPurse.AddCoins(GenerateCoins(CoinType.Platinum, platinumCount));
            coinPurse.AddCoins(GenerateCoins(CoinType.Diamond, diamondCount));

            return coinPurse;
        }

        private string GenerateUpdateQuery(CoinPurse entity)
        {
            return _updateCoinPurseQuery;
        }
    }
}
