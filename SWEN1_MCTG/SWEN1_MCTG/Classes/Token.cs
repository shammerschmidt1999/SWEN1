using System;
using System.Collections.Generic;
using SWEN1_MCTG.Classes.HttpSvr;

namespace SWEN1_MCTG.Classes
{
    public static class Token
    {
        private const string _ALPHABET = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static ITokenRepository _tokenRepository;

        public static void Initialize(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Create a token for a user
        /// </summary>
        /// <param name="user"> User entity </param>
        /// <returns> Token string </returns>
        internal static string _CreateTokenFor(User user)
        {
            string rval = string.Empty;
            Random rnd = new();

            for (int i = 0; i < 24; i++)
            {
                rval += _ALPHABET[rnd.Next(0, 62)];
            }

            _tokenRepository.CreateToken(rval, user.Id);

            return rval;
        }

        public static (bool Success, User? User) Authenticate(string token)
        {
            return _tokenRepository.Authenticate(token);
        }

        /// <summary>
        /// Authenticate a user based on the Authorization header
        /// </summary>
        /// <param name="e"> HTTP Server Args </param>
        /// <returns> TRUE and a User entity if successfully authenticated; FALSE and null else</returns>
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