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
    public class StatsHandler : Handler, IHandler
    {
        private readonly string _connectionString;
        private readonly IUserRepository _userRepository;

        public StatsHandler()
        {
            _connectionString = AppSettings.GetConnectionString("TestConnection");
            _userRepository = new UserRepository(_connectionString);
        }

        /// <summary>
        /// Handles user creation and login
        /// </summary>
        /// <param name="e"> Console arguments </param>
        /// <returns> bool if request was successful or not </returns>
        public override async Task<bool> HandleAsync(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/stats") && (e.Method == "GET"))
            {
                return await _DisplayStatsAsync(e);
            }

            return false;
        }

        private async Task<bool> _DisplayStatsAsync(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

                if (ses.Success)
                { 
                    status = HttpStatusCode.OK;
                    reply = new JsonObject()
                        {
                            ["success"] = true,
                            ["Username"] = ses.User.Username,
                            ["Elo"] = ses.User.Elo,
                            ["Wins"] = ses.User.Wins,
                            ["Defeats"] = ses.User.Defeats,
                            ["Draws"] = ses.User.Draws
                    };
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

            // Use indented formatting for the JSON response
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            e.Reply(status, reply?.ToJsonString(options));
            return true;
        }


        private JsonArray _generateScoreboardArray(List<(string Username, int Elo)> users)
        {
            JsonArray array = new JsonArray();
            int rank = 1;

            foreach (var user in users)
            {
                JsonObject obj = new JsonObject()
                {
                    ["rank"] = rank,
                    ["username"] = user.Username,
                    ["elo"] = user.Elo
                };

                rank++;
                array.Add(obj);
            }

            return array;
        }

    }
}
