using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    //For encrypt and decrypt something on the server
    private readonly SymmetricSecurityKey _key;


    public TokenService(IConfiguration config)
    {
        //Encode the key into bytes format
        //Save the TokenKey in the appsettings.Development.json
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }

    public string CreateToken(AppUser user)
    {
        //Claim about a user, i might claim my name is something
        //It is a bit of information that a user claims, we set the claim to be the userId
        //Claim needs to be String 
        //We create a list of claim for many claims 
        var claims = new List<Claim>
            {
                //To get the userId with token
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

        //Sign the claims, SecurityAlgorithms... is used to encrypt this key
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        //What our token going to include 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //claims we going to return, expires is the expiry for the token(7 days)
            //also the signingCredentials
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        //Save our packages
        var tokenHandler = new JwtSecurityTokenHandler();
        //To create our token with the claims etc
        var token = tokenHandler.CreateToken(tokenDescriptor);
        //Return the token when the user login or register
        return tokenHandler.WriteToken(token);
    }
}