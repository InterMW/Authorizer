using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Authorizer.Domain;
using System.Security.Cryptography;
using System.Text;

namespace DomainService;
public interface IAuthorizerDomainService
{
    string Login(string username, string password);
    Task<bool> IsValid(string token);
}

public class AuthorizerDomainService(IOptions<JWTOptions> jwtOptions): IAuthorizerDomainService
{
    public string Login(string username, string password)
    {
        if (username != "jbmelberg" || !CheckHash(password))
        {
            return string.Empty;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
          new Claim(JwtRegisteredClaimNames.Sub, username),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim("Elevation", "Admin")
        };

        var token = new JwtSecurityToken(
            jwtOptions.Value.Issuer,
            jwtOptions.Value.Issuer,
            claims,
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> IsValid(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var result = await handler.ValidateTokenAsync(token, GetValidationParameters());

        return result?.IsValid ?? false;
    }

    private bool CheckHash(string password)
    {
        var s = SHA256.Create();

        byte[] data = s.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString() == jwtOptions.Value.Admin;
    }

    private TokenValidationParameters GetValidationParameters() =>
    new TokenValidationParameters()
    {
        LifetimeValidator = CustomLifetimeValidator,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Value.Issuer,
        ValidAudience = jwtOptions.Value.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key)) // The same key as the one that generate the token
    };

    public static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters validationParameters)
    {
        if (DateTime.UtcNow.AddMinutes(-120)  > notBefore)
        {
          return false;
        }

        if (DateTime.UtcNow > expires)
        {
          return false;
        }

        return true;
    }
}

