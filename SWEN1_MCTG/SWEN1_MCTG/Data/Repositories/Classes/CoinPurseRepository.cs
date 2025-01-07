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

        /// <summary>
        /// Gets the users coinpurse
        /// </summary>
        /// <param name="userId"> The Id of the user </param>
        /// <returns> A coinPurse entity that matches the users coinpurse </returns>
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

        /// <summary>
        /// Deletes a users CoinPurse
        /// </summary>
        /// <param name="userId"> The users Id </param>
        public void DeleteByUserId(int userId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_deleteCoinPurseQuery, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates a users CoinPurse
        /// </summary>
        /// <param name="coinPurse"> The CoinPurse entity of the user </param>
        public void UpdateCoinPurse(CoinPurse coinPurse)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_updateCoinPurseQuery, connection);
            AddParameters(command, coinPurse);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a new CoinPurse to the Database
        /// </summary>
        /// <param name="coinPurse"> The coinPurse entity to be added to the DB </param>
        public void AddCoinPurse(CoinPurse coinPurse)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(_insertCoinPurseQuery, connection);
            AddParameters(command, coinPurse);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Method to add Coins to a users coinPurse
        /// </summary>
        /// <param name="userId"> The users Id </param>
        /// <param name="coins"> The amount and kind of coins to be added </param>
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

        /// <summary>
        /// Removes Coins from the users coinPurse by determining the value of the users combined coins
        /// </summary>
        /// <param name="userId"> The users Id </param>
        /// <param name="amount"> The cost and value that should be removed from the coinpurse </param>
        /// <returns> TRUE if operation successful; FALSE if unsuccessful </returns>
        public bool RemoveCoinsFromPurse(int userId, int amount)
        {
            CoinPurse coinPurse = GetByUserId(userId);
            if (coinPurse == null)
            {
                return false; // CoinPurse not found
            }

            List<Coin> coins = coinPurse.Coins.OrderByDescending(c => c.Value).ToList();
            int remainingAmount = amount;

            foreach (Coin coin in coins)
            {
                if (remainingAmount <= 0)
                    break;

                if (coin.Value <= remainingAmount)
                {
                    remainingAmount -= coin.Value;
                    coinPurse.Coins.Remove(coin);
                }
                else
                {
                    coinPurse.Coins.Remove(coin);
                    int remainingValue = coin.Value - remainingAmount;
                    remainingAmount = 0;

                    // Convert remaining value into smaller denomination coins
                    foreach (CoinType coinType in Enum.GetValues(typeof(CoinType)).Cast<CoinType>().OrderByDescending(ct => (int)ct))
                    {
                        while (remainingValue >= (int)coinType)
                        {
                            coinPurse.AddCoin(new Coin(coinType));
                            remainingValue -= (int)coinType;
                        }
                    }
                }
            }

            if (remainingAmount > 0)
            {
                return false; // Not enough coins
            }

            UpdateCoinPurse(coinPurse);
            return true;
        }

        /// <summary>
        /// Removes all coins from a users coinpurse
        /// </summary>
        /// <param name="userId"> The users Id </param>
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
