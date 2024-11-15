using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

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
            if (e.Path.StartsWith("/users/create")) // Creates user in memory
            {
                if (e.Method == "POST")
                {
                    var user = JsonSerializer.Deserialize<User>(e.Payload);
                    if (user != null)
                    {
                        User newUser = new User(user.Username, user.Password);
                        Console.WriteLine($"User created: {user.Username}, Password: {user.Password}");
                        e.Reply(HttpStatusCode.OK, "User created successfully");
                        Console.WriteLine("-------- USER PROFILE --------");
                        user.PrintUser();
                    }
                    else
                    {
                        e.Reply(HttpStatusCode.BAD_REQUEST, "Invalid user data");
                        return false;
                    }
                }

                return true;
            }

            if (e.Path.StartsWith("/users/sessions")) // Logs user in
            {
                if (e.Method == "POST")
                {
                    // Implement user in memory login
                }
            }
            return false;
        }
    }
}
