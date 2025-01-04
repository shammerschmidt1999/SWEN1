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

        public CoinPurseRepository() : base(null, null)
        {
        }
        public CoinPurseRepository(string connectionString)
            : base(connectionString, "coin_purses")
        {
        }

        // Override to create a CoinPurse from the database values
        protected override CoinPurse CreateEntity()
        {
            return new CoinPurse();
        }

        // Implement MapReaderToEntity to convert DB rows into CoinPurse
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

        // Helper method to generate coins for a given coin type and count
        public List<Coin> GenerateCoins(CoinType coinType, int count)
        {
            List<Coin> coins = new List<Coin>();
            for (int i = 0; i < count; i++)
            {
                coins.Add(new Coin(coinType));
            }
            return coins;
        }

        // Override to generate the insert query for the CoinPurse
        protected override string GenerateInsertQuery(CoinPurse coinPurse)
        {
            return _insertCoinPurseQuery;
        }

        // Override to add parameters to the insert query
        protected override void AddParameters(NpgsqlCommand command, CoinPurse coinPurse)
        {
            command.Parameters.AddWithValue("@UserId", coinPurse.UserId);
            command.Parameters.AddWithValue("@Bronze", coinPurse.Coins.Count(c => c.CoinType == CoinType.Bronze));
            command.Parameters.AddWithValue("@Silver", coinPurse.Coins.Count(c => c.CoinType == CoinType.Silver));
            command.Parameters.AddWithValue("@Gold", coinPurse.Coins.Count(c => c.CoinType == CoinType.Gold));
            command.Parameters.AddWithValue("@Platinum", coinPurse.Coins.Count(c => c.CoinType == CoinType.Platinum));
            command.Parameters.AddWithValue("@Diamond", coinPurse.Coins.Count(c => c.CoinType == CoinType.Diamond));
        }

        // Method to get CoinPurse by UserId
        public CoinPurse GetByUserId(int userId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_getCoinPurseByIdQuery, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapReaderToEntity(reader);
            }

            return null; // Return null if the CoinPurse is not found for the user
        }

        // Method to delete CoinPurse by UserId
        public void DeleteByUserId(int userId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_deleteCoinPurseQuery, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.ExecuteNonQuery();
        }

        // Additional method to update a user's coin purse
        public void UpdateCoinPurse(CoinPurse coinPurse)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_updateCoinPurseQuery, connection);
            AddParameters(command, coinPurse);
            command.ExecuteNonQuery();
        }

        // Method to add a new CoinPurse
        public void AddCoinPurse(CoinPurse coinPurse)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_insertCoinPurseQuery, connection);
            AddParameters(command, coinPurse);
            command.ExecuteNonQuery();
        }

        // Method to add coins to the CoinPurse (handles adding multiple coins)
        public void AddCoinsToPurse(int userId, IEnumerable<Coin> coins)
        {
            CoinPurse coinPurse = GetByUserId(userId);
            if (coinPurse == null)
            {
                coinPurse = new CoinPurse { UserId = userId };
                AddCoinPurse(coinPurse);
            }

            coinPurse.AddCoins(coins);
            UpdateCoinPurse(coinPurse);
        }

        // Method to remove coins from the CoinPurse (handles removing coins by type)
        public bool RemoveCoinsFromPurse(int userId, CoinType coinType, int count)
        {
            CoinPurse coinPurse = GetByUserId(userId);
            if (coinPurse == null)
            {
                return false; // CoinPurse not found
            }

            bool result = coinPurse.RemoveCoins(coinType, count);
            if (result)
            {
                UpdateCoinPurse(coinPurse);
            }

            return result;
        }

        // Method to remove all coins from the CoinPurse
        public void RemoveAllCoinsFromPurse(int userId)
        {
            CoinPurse coinPurse = GetByUserId(userId);
            if (coinPurse == null)
            {
                return; // CoinPurse not found
            }

            coinPurse.Coins.Clear();
            UpdateCoinPurse(coinPurse);
        }
    }
}
