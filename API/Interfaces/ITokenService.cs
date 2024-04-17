using API.Entities;

namespace API.Interfaces;

//Any other class that implements this interface has to support this method 
//and it has to return a string and take an appuser as an argument
public interface ITokenService
{
    string CreateToken(AppUser user);
}