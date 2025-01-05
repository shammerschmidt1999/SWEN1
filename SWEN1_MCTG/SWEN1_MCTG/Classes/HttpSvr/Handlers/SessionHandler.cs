using SWEN1_MCTG.Interfaces;
using System;
using System.Text.Json.Nodes;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public class SessionHandler : Handler, IHandler
    {
        /// <summary>
        /// Handles session creation
        /// </summary>
        /// <param name="e"> Server Event Arguments </param>
        /// <returns> True (via _CreateSession) on successful session create, false on unsuccessful session create </returns>
        public override bool Handle(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/sessions") && (e.Method == "POST"))
            {
                return _CreateSession(e);
            }

            return false;
        }

        /// <summary>
        /// Logs in a user by creating a session and returning a token
        /// </summary>
        /// <param name="e"> Server Event Arguments </param>
        /// <returns> True on successful session creation, false on unsuccessful session creation </returns>
        public static bool _CreateSession(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;

            try
            {
                JsonNode? json = JsonNode.Parse(e.Payload);
                if (json != null)
                {
                    (bool Success, string Token) result = User.Logon((string)json["Username"]!, (string)json["Password"]!);

                    if (result.Success)
                    {
                        status = HttpStatusCode.OK;
                        reply = new JsonObject()
                        {
                            ["success"] = true,
                            ["message"] = "Login successful.",
                            ["token"] = result.Token
                        };
                    }
                    else
                    {
                        status = HttpStatusCode.UNAUTHORIZED;
                        reply = new JsonObject()
                        {
                            ["success"] = false,
                            ["message"] = "Login failed"
                        };
                    }
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
