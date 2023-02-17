using BE.Data.Models;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace BE.Helpers
{
    public class TokenHelper
    {
        public class Token
        {
            public int User { get; set; }
            public int Group { get; set; }
        }
        public static Token GetUserId(ClaimsPrincipal user)
        {
            Token token = new Token();
            // Return 0 if can't get id user in token
            var claimsUser = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            var claimsGroup = user.Claims.FirstOrDefault(x => x.Type.ToString().Equals("IdGroup", StringComparison.InvariantCultureIgnoreCase));

            if (claimsUser != null)
            {
                token.User = int.Parse(claimsUser.Value);
            }
            else
            {
                token.User = 0;
            }
            if(claimsGroup != null)
            {
                token.Group = int.Parse(claimsGroup.Value);
            }
            else
            {
                token.Group = 0;
            }

            return token;
        }
    }
}
