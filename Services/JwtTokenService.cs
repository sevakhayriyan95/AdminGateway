using AdminGateway.Entities;
using AdminGateway.Infrastructure;
using AdminGateway.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace AdminGateway.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly AdminDbContext _dbContext;
        private readonly SigningCredentials _credentials;

        public JwtTokenService(IConfiguration configuration, AdminDbContext dbContext, SigningCredentials credentials)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _credentials = credentials;
        }

        public string GetAccessToken(TokenData tokenData)
        {
            string issuer = _configuration["AdminAuth:JWT:Issuer"];
            string audience = _configuration["AdminAuth:JWT:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = new Dictionary<string, object>
                {
                    {JwtRegisteredClaimNames.Sid, JsonConvert.SerializeObject(tokenData) },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
                },
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = _credentials
            };

            return CreateToken(tokenDescriptor);
        }

        public async Task<string> GetRefreshToken(TokenData tokenData)
        {
            string issuer = _configuration["AdminAuth:JWT:RefreshIssuer"];
            string audience = _configuration["AdminAuth:JWT:RefreshAudience"];
            var expireDate = DateTime.UtcNow.AddDays(90);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Claims = new Dictionary<string, object>
                {
                    {JwtRegisteredClaimNames.Sid, JsonConvert.SerializeObject(tokenData)},
                    {JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()}
                },
                Expires = expireDate,
                SigningCredentials = _credentials
            };

            string token = CreateToken(tokenDescriptor);

            await _dbContext.RefreshToken.AddAsync(new RefreshToken()
            {
                UserId = tokenData.UserId,
                Token = token,
                IsRevoked = false,
                ExpireDate = expireDate
            });
            await _dbContext.SaveChangesAsync();

            return token;
        }

        public IIdentity UserIdentity(string token)
        {
            try
            {
                token = token.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (principal?.Identity?.IsAuthenticated ?? false)
                {
                    return principal.Identity;
                }

                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public TokenData ExtractDataFromToken(IIdentity? identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;

            var json = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid);

            if (json != null)
            {
                return JsonConvert.DeserializeObject<TokenData>(json.Value);
            }

            return null;
        }

        private string CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private TokenValidationParameters GetValidationParameters()
        {
            string issuer = _configuration["AdminAuth:JWT:Issuer"];
            string audience = _configuration["AdminAuth:JWT:Audience"];
            string key = _configuration["AdminAuth:JWT:SigningKey"];

            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    }
}
