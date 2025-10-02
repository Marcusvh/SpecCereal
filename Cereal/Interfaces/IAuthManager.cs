using System.Security.Claims;

namespace Cereal.Interfaces
{
    public interface IAuthManager
    {
        ClaimsPrincipal ValidateUser(string username, string password);
        bool CreateUser(string username, string password, string role = "basic");
    }
}
