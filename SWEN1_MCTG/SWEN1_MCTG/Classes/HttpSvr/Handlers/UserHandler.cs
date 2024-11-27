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

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public class UserHandler : Handler, IHandler
    {
        /// <summary>
        /// Handles user creation and login
        /// </summary>
        /// <param name="e"> Console arguments </param>
        /// <returns> bool if request was successful or not </returns>
        public override bool Handle(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/users") && (e.Method == "POST"))
            {                                                                   
                return _CreateUser(e);
            }
            else if (e.Path.StartsWith("/users/") && (e.Method == "GET"))        
            {
                return _QueryUser(e);
            }

            return false;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="e"> Server Event Arguments </param>
        /// <returns> True on successful user creation, false on unsuccessful user creation </returns>
        private static bool _CreateUser(HttpSvrEventArgs e)
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
                    if (User.Exists(username))
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
                        User.Create(username, password);
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
        private static bool _QueryUser(HttpSvrEventArgs e)
        {
            JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
            int status = HttpStatusCode.BAD_REQUEST;                         

            try
            {
                (bool Success, User? User) ses = Token.Authenticate(e);        

                if (ses.Success)
                {                                                              
                    User? user = User.Get(e.Path[7..]);                      

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
    
    }
}
