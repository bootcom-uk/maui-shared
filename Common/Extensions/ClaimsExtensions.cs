using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Extensions
{
    public static class ClaimsExtensions
    {

        public static IEnumerable<Claim> ExtractClaims(this string jwtToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
            IEnumerable<Claim> claims = securityToken.Claims;
            return claims;
        }

    }
}
