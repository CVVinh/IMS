using BE.Data.Contexts;
using BE.Data.Models;
using BE.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BE.Services.TokenServices
{
    public class TokenService:ITokenService
    {
        private readonly AppDbContext _context;
        private readonly JwtSetting _appSettings;
        private readonly IConfiguration _iconfiguration;

        public TokenService(AppDbContext context, IOptionsMonitor<JwtSetting> optionsMonitor, IConfiguration iconfiguration)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
            _iconfiguration = iconfiguration;
        }

        public string GenerateToken(Users userS)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secret);

            Claim[] getClaims()
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, userS.email));
                claims.Add(new Claim("userCode", userS.userCode));
                claims.Add(new Claim("id", userS.id.ToString()));
                claims.Add(new Claim("IdGroup", userS.IdGroup.ToString()));

                if (userS.avatarLink != null)
                {
                    claims.Add(new Claim("AvatarLink", userS.avatarLink.ToString()));
                }

                if (getPermission_Use_Menu(userS.id) != null)
                {
                    foreach (var item in getPermission_Use_Menu(userS.id))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }
                }
                if (getPermission_Group_AccessModule(userS.id) != null)
                {
                    foreach (var item in getPermission_Group_AccessModule(userS.id))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }
                }

                if (getPermission_By_Group_String(userS.id) != null)
                {
                    foreach (var item in getPermission_By_Group_String(userS.id))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
                    }
                }

                if (getPermission_By_Group_Int(userS.id) != null)
                {
                    foreach (var item in getPermission_By_Group_Int(userS.id))
                    {
                        claims.Add(new Claim("listGroup", item.ToString()));
                    }
                }

                return claims.ToArray();
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(getClaims()),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpriredToken(string token)
        {
            var jwtSetting = Encoding.UTF8.GetBytes(_iconfiguration.GetSection("JwtSetting:Secret").Value);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSetting),
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secret);
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        /*private string GetRole(Users user)
        {
            var role = "Anonymous";
            foreach (eRole erole in (eRole[])Enum.GetValues(typeof(eRole)))
            {
                if (user.IdGroup == (int)erole)
                {
                    role = erole.ToString();
                }
            }
            return role;
        }*/
        public ClaimToken Decode(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenS = jwtSecurityToken as JwtSecurityToken;
            var userCode = tokenS.Claims.First(claim => claim.Type == "userCode").Value;
            var id = tokenS.Claims.First(claim => claim.Type == "id").Value;
            var group = tokenS.Claims.First(claim => claim.Type == "IdGroup").Value;
            var key = tokenS.Claims.First(claim => claim.Type == "Key").Value;
            var response = new ClaimToken
            {
                userCode = userCode,
                id = Convert.ToInt32(id),
                group = Convert.ToInt32(group)
            };
            return response;
        }

        // bat theo use_menu add edit delete export = 1
        private List<string> getPermission_Use_Menu(int idUser)
        {
            var query = from a in _context.Permission_Use_Menus
                        join b in _context.modules
                        on a.idModule equals b.id
                        where a.IdUser==idUser
                        select new {a, b};
            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                    data.Add("modules: "+permission.b.nameModule +" "+ "add: "+permission.a.Add);

                    data.Add("modules: " + permission.b.nameModule + " " + "update: " + permission.a.Update);

                    data.Add("modules: " + permission.b.nameModule + " " + "delete: " + permission.a.Delete);

                    data.Add("modules: " + permission.b.nameModule + " " + "deleteMulti: " + permission.a.DeleteMulti);

                    data.Add("modules: " + permission.b.nameModule + " " + "confirm: " + permission.a.Confirm);

                    data.Add("modules: " + permission.b.nameModule + " " + "confirmMulti: " + permission.a.ConfirmMulti);

                    data.Add("modules: " + permission.b.nameModule + " " + "refuse: " + permission.a.Refuse);

                    data.Add("modules: " + permission.b.nameModule + " " + "addMember: " + permission.a.AddMember);

                    data.Add("modules: " + permission.b.nameModule + " " + "export: " + permission.a.Export);
                }
                return data;
            }
            return null!;
        }

        private List<string> getPermission_By_Group_String(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.id == idUser
                        select d.NameGroup.ToLower();

            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission in query)
                {
                   data.Add(permission);
                }
                return data;
            }
            return null!;
        }

        // bat lam duoc nhung gi
        private List<int> getPermission_By_Group_Int(int idUser)
        {
            var query = from u in _context.Users
                        join g in _context.UserGroups on u.id equals g.idUser
                        join d in _context.Groups on g.idGroup equals d.Id
                        where u.id == idUser
                        select d.Id;

            if (query.Count() != 0)
            {
                List<int> data = new List<int>();
                foreach (var permission in query)
                {
                    data.Add(permission);
                }
                return data;
            }
            return null!;
        }

        // Bat co quyen hay khong
        private List<string> getPermission_Group_AccessModule(int idUser)
        {
            var query = from a in _context.Permission_Groups
                        join b in _context.modules
                        on a.IdModule equals b.id
                        join c in _context.Users
                        on a.IdGroup equals c.IdGroup
                        where c.id == idUser
                        select new { a, b };
            if (query.Count() != 0)
            {
                List<string> data = new List<string>();
                foreach (var permission_group in query)
                {
                    data.Add("permission_group: " + permission_group.a.Access+
                        " module: "+permission_group.b.nameModule);

                }
                return data;
            }
            return null!;
        }

    }
}
