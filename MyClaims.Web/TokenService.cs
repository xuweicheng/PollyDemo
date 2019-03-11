using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyClaims.Web
{
    public class TokenService : ITokenService
    {
        private static string token;

        public TokenService()
        {
            token = "invalid_token";
        }

        public string GetToken()
        {
            //cache management
            return token;
        }

        public string RefreshToken()
        {
            token = "valid_token";
            return token;
        }
    }
}
