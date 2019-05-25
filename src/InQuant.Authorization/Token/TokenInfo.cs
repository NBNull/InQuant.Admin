using InQuant.Authorization.Permissions;
using System.Collections.Generic;

namespace InQuant.Authorization.Token
{
    public class TokenInfo
    {
        public TokenInfo() { }
        
        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool IsAdmin { get; set; }
    }
}
