using BE.Data.Models;
using System.Security.Claims;

namespace BE.Services.TokenServices
{
    public interface ITokenService
    {
        string GenerateToken(Users users);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpriredToken(string token);
        ClaimToken Decode(string token);
    }
    public class ClaimToken {
        public string userCode { get; set; }
        public int id { get; set; }
        public int group { get; set; }
        

    } 
}
