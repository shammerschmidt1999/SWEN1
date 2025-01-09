using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes.HttpSvr;
using SWEN1_MCTG.Data.Repositories.Interfaces;

namespace SWEN1_MCTG.Classes
{
    public static class Token
    {
        private const string _ALPHABET = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static ITokenRepository? _tokenRepository;

        public static void Initialize(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Create a token for a user
        /// </summary>
        /// <param name="user"> User entity </param>
        /// <returns> Token string </returns>
        internal static async Task<string> _CreateTokenForAsync(User user)
        {
            string rval = string.Empty;
            Random rnd = new();

            for (int i = 0; i < 24; i++)
            {
                rval += _ALPHABET[rnd.Next(0, 62)];
            }

            if (_tokenRepository != null)
            {
                await _tokenRepository.CreateTokenAsync(rval, user.Id);
            }

            return rval;
        }

        /// <summary>
        /// Authenticates the user by looking for user token relationship in DB
        /// </summary>
        /// <param name="token"> Token string </param>
        /// <returns> TRUE and the user entity if successful; FALSE and null if not </returns>
        public static async Task<(bool Success, User? User)> AuthenticateTokenAsync(string token)
        {
            if (_tokenRepository != null)
            {
                return await _tokenRepository.AuthenticateAsync(token);
            }
            return (false, null);
        }

        /// <summary>
        /// Authenticate a user based on the Authorization header
        /// </summary>
        /// <param name="e"> HTTP Server Args </param>
        /// <returns> TRUE and a User entity if successfully authenticated; FALSE and null else</returns>
        public static async Task<(bool Success, User? User)> AuthenticateBearerAsync(HttpSvrEventArgs e)
        {
            foreach (HttpHeader i in e.Headers)
            {
                if (i.Name == "Authorization")
                {
                    if (i.Value.StartsWith("Bearer "))
                    {
                        return await AuthenticateTokenAsync(i.Value[7..].Trim());
                    }
                    break;
                }
            }

            return (false, null);
        }
    }
}
