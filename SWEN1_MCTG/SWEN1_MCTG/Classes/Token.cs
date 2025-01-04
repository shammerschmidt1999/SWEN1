using SWEN1_MCTG.Classes.HttpSvr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Classes
{
    public static class Token
    {
        // Alphabet string
        private const string _ALPHABET = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Token dictionary
        internal static Dictionary<string, User> _Tokens = new();

        /// <summary> Creates a new token for a user </summary>
        /// <param name="user"> User that the token gets assigned to </param>
        /// <returns> Token string </returns>
        internal static string _CreateTokenFor(User user)
        {
            string rval = string.Empty;
            Random rnd = new();

            for (int i = 0; i < 24; i++)
            {
                rval += _ALPHABET[rnd.Next(0, 62)];
            }

            _Tokens.Add(rval, user);

            return rval;
        }

        /// <summary> Authenticates a user by token </summary>
        /// <param name="token"> Token string </param>
        /// <returns> Returns a tuple of success flag and user object.
        ///          If successful, the success flag is TRUE and the user represents the authenticated user,
        ///          otherwise success flag if FALSE and user object is NULL</returns>
        public static (bool Success, User? User) Authenticate(string token)
        {
            if (Program.ALLOW_DEBUG_TOKEN && token.EndsWith("-debug"))
            {
                token = token[..^6];
                User? user = User.Get(token);

                return ((user != null), user);
            }

            if (_Tokens.ContainsKey(token))
            { 
                return (true, _Tokens[token]);
            }

            return (false, null);
        }


        /// <summary> Authenticates a user by token </summary>
        /// <param name="e"> Event arguments </param>
        /// <returns> Returns a tuple of success flag and user object
        ///          If successful, the success flag is TRUE and the user represents the authenticated user,
        ///          otherwise success flag if FALSE and user object is NULL </returns>
        public static (bool Success, User? User) Authenticate(HttpSvrEventArgs e)
        {
            foreach (HttpHeader i in e.Headers)
            {
                if (i.Name == "Authorization")
                {
                    if (i.Value[..7] == "Bearer ")
                    {
                        return Authenticate(i.Value[7..].Trim());
                    }
                    break;
                }
            }

            return (false, null);
        }
    }
}
