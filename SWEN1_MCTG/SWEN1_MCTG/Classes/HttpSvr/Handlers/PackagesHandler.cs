using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Classes.Exceptions;
using System.Text.Json.Nodes;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public class PackagesHandler : Handler, IHandler
    {
        private readonly string _connectionString;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly PackageService _packageService;

        public PackagesHandler()
        {
            _connectionString = AppSettings.GetConnectionString("TestConnection");
            _cardRepository = new CardRepository(_connectionString);
            _userRepository = new UserRepository(_connectionString);
            _packageService = new PackageService();
        }

        /// <summary>
        /// Handles package purchase
        /// </summary>
        /// <param name="e"> Console arguments </param>
        /// <returns> bool if request was successful or not </returns>
        public override async Task<bool> HandleAsync(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/transactions/packages") && (e.Method == "POST"))
            {
                return await _PurchasePackageAsync(e);
            }
            return false;
        }

        /// <summary>
        /// Method to purchase a package
        /// </summary>
        /// <param name="e"> HttpSvrEventArgs </param>
        /// <returns> TRUE on success; FALSE on failure </returns>
        private async Task<bool> _PurchasePackageAsync(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

                if (ses.Success)
                {
                    JsonNode? json = JsonNode.Parse(e.Payload);
                    if (json != null)
                    {
                        GlobalEnums.PackageType packageType = 
                            (GlobalEnums.PackageType)Enum.Parse(typeof(GlobalEnums.PackageType),
                                (string)json["packageType"]!);

                        User user = await _userRepository.GetByUsernameAsync(ses.User!.Username);
                        await _packageService.PurchasePackageAsync(user.Id, packageType);

                        status = HttpStatusCode.OK;
                        reply = new JsonObject()
                        {
                            ["success"] = true,
                            ["message"] = "Package purchased successfully.",
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
                reply = new JsonObject() { ["success"] = false, ["message"] = "Not enough coins." };
            }

            e.Reply(status, reply?.ToJsonString());
            return true;
        }
    }
}
