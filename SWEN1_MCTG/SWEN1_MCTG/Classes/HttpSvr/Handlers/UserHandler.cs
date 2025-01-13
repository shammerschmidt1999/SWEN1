using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Classes.Exceptions;
using System.Text.Json.Nodes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public class UserHandler : Handler, IHandler
    {
        private readonly string _connectionString;
        private readonly IUserRepository _userRepository;
        private readonly ICoinPurseRepository _coinPurseRepository;
        private readonly IStackRepository _stackRepository;

        public UserHandler()
        {
            _connectionString = AppSettings.GetConnectionString("TestConnection");
            _userRepository = new UserRepository(_connectionString);
            _coinPurseRepository = new CoinPurseRepository(_connectionString);
            _stackRepository = new StackRepository(_connectionString);
        }

        /// <summary>
        /// Handles user creation and login
        /// </summary>
        /// <param name="e"> Console arguments </param>
        /// <returns> bool if request was successful or not </returns>
        public override async Task<bool> HandleAsync(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/users") && (e.Method == "POST"))
            {
                return await _CreateUserAsync(e);
            }
            else if (e.Path.StartsWith("/users/") && (e.Method == "GET"))
            {
                return await _QueryUserAsync(e);
            }
            else if (e.Path.StartsWith("/users/") && (e.Method == "PUT"))
            {
                // Match the path pattern for user data
                Match match = Regex.Match(e.Path.TrimEnd('/', ' ', '\t'), @"^/users/(?<username>[^/]+)$");
                if (match.Success)
                {
                    string username = match.Groups["username"].Value;
                    return await _UpdateUserAsync(e, username);
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="e"> Server Event Arguments </param>
        /// <returns> True on successful user creation, false on unsuccessful user creation </returns>
        private async Task<bool> _CreateUserAsync(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                JsonNode? json = JsonNode.Parse(e.Payload);
                if (json != null)
                {
                    string username = (string)json["Username"]!;
                    string password = (string)json["Password"]!;

                    // Check if the user already exists
                    if (await _userRepository.ExistsAsync(username))
                    {
                        status = HttpStatusCode.BAD_REQUEST;
                        reply = new JsonObject()
                        {
                            ["success"] = false,
                            ["message"] = "User already exists."
                        };
                    }
                    else
                    {
                        // Create the new user
                        User newUser = new User(username, password);
                        await _userRepository.AddAsync(newUser);
                        User addedUser = await _userRepository.GetByUsernameAsync(username);

                        CoinPurse coinPurse = new CoinPurse(GlobalEnums.CoinType.Diamond)
                        {
                            UserId = addedUser.Id
                        };
                        await _coinPurseRepository.AddAsync(coinPurse);

                        status = HttpStatusCode.OK;
                        reply = new JsonObject()
                        {
                            ["success"] = true,
                            ["message"] = "User created successfully."
                        };
                    }
                }
            }
            catch (UserException ex)
            {
                reply = new JsonObject() { ["success"] = false, ["message"] = ex.Message };
            }
            catch (Exception)
            {
                reply = new JsonObject() { ["success"] = false, ["message"] = "Unexpected error." };
            }

            e.Reply(status, reply?.ToJsonString());
            return true;
        }

        /// <summary>
        /// Gets user information
        /// </summary>
        /// <param name="e"> Server Event Arguments </param>
        /// <returns> True on successful query, false on unsuccessful query </returns>
        private async Task<bool> _QueryUserAsync(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

                if (ses.Success)
                {
                    User? user = await _userRepository.GetByUsernameAsync(e.Path[7..]);
                    Stack userStack = await _stackRepository.GetByUserIdAsync(ses.User!.Id);
                    CoinPurse userCoinPurse = await _coinPurseRepository.GetByUserIdAsync(ses.User!.Id);

                    if (user == null)
                    {
                        status = HttpStatusCode.NOT_FOUND;
                        reply = new JsonObject() { ["success"] = false, ["message"] = "User not found." };
                    }
                    else
                    {
                        status = HttpStatusCode.OK;
                        reply = new JsonObject()
                        {
                            ["success"] = true,
                            ["username"] = user!.Username,
                            ["cardAmount"] = userStack!.Cards.Count,
                            ["coinAmount"] = userCoinPurse.Coins.Count,
                            ["coinsValue"] = userCoinPurse!.GetCoinsValue(),
                            ["elo"] = user!.Elo,
                            ["wins"] = user!.Wins,
                            ["defeats"] = user!.Defeats,
                            ["draws"] = user!.Draws
                        };
                    }
                }
                else
                {
                    status = HttpStatusCode.UNAUTHORIZED;
                    reply = new JsonObject() { ["success"] = false, ["message"] = "Unauthorized." };
                }
            }
            catch (Exception)
            {
                reply = new JsonObject() { ["success"] = false, ["message"] = "Unexpected error." };
            }

            e.Reply(status, reply?.ToJsonString());
            return true;
        }

        private async Task<bool> _UpdateUserAsync(HttpSvrEventArgs e, string username)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

                if (ses.Success)
                {
                    User? user = await _userRepository.GetByUsernameAsync(username);

                    if (user == null)
                    {
                        status = HttpStatusCode.NOT_FOUND;
                        reply = new JsonObject() { ["success"] = false, ["message"] = "User not found." };
                    }
                    else
                    {
                        JsonNode? json = JsonNode.Parse(e.Payload);
                        if (json != null)
                        {
                            string? newUsername = json["Username"]?.ToString();
                            string? newPassword = json["Password"]?.ToString();
                            JsonObject? newCoins = json["Coins"]?.AsObject();

                            if (!string.IsNullOrEmpty(newUsername))
                            {
                                user.changeUsername(newUsername);
                            }
                            if (!string.IsNullOrEmpty(newPassword))
                            {
                                user.changePassword(newPassword);
                            }
                            if (newCoins != null)
                            {
                                CoinPurse userCoinPurse = await _coinPurseRepository.GetByUserIdAsync(user.Id);
                                userCoinPurse.Coins.Clear();

                                foreach (var coinType in newCoins)
                                {
                                    if (Enum.TryParse(coinType.Key, out GlobalEnums.CoinType type))
                                    {
                                        int amount = coinType.Value.GetValue<int>();
                                        for (int i = 0; i < amount; i++)
                                        {
                                            userCoinPurse.AddCoin(new Coin(type));
                                        }
                                    }
                                }
                                await _coinPurseRepository.UpdateCoinPurseAsync(userCoinPurse);
                            }

                            await _userRepository.UpdateAsync(user);

                            status = HttpStatusCode.OK;
                            reply = new JsonObject() { ["success"] = true, ["message"] = "User updated successfully." };
                        }
                    }
                }
                else
                {
                    status = HttpStatusCode.UNAUTHORIZED;
                    reply = new JsonObject() { ["success"] = false, ["message"] = "Unauthorized." };
                }
            }
            catch (Exception)
            {
                reply = new JsonObject() { ["success"] = false, ["message"] = "Unexpected error." };
            }

            e.Reply(status, reply?.ToJsonString());
            return true;
        }
    }
}
