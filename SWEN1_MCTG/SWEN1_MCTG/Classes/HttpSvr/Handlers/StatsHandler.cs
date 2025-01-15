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

        /// <summary>
        /// Displays the stats of the user
        /// </summary>
        /// <param name="e"> HttpSvrEventArgs </param>
        /// <returns> TRUE if operation was successful; FALSE on failure </returns>
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
                    reply = _GenerateStats(ses.User);
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

        /// <summary>
        /// Generates a JSON reply for the user stats
        /// </summary>
        /// <param name="user"> User entity </param>
        /// <returns> JSONObject containing user data </returns>
        private JsonObject _GenerateStats(User user)
        {
            JsonObject reply = new JsonObject()
            {
                ["success"] = true,
                ["Username"] = user.Username,
                ["Elo"] = user.Elo,
                ["Wins"] = user.Wins,
                ["Defeats"] = user.Defeats,
                ["Draws"] = user.Draws
            };
            return reply;
        }

    }
}
