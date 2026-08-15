using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface ITokenService
{
    TokenResult CreateToken(SystemUser user);
}
