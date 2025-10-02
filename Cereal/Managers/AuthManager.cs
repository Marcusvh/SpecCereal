using Cereal.Context;
using Cereal.Interfaces;
using CerealLib;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;

namespace Cereal.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly CerealContext _context;

        public AuthManager(CerealContext context)
        {
            _context = context;
        }

        public bool CreateUser(string username, string plainPassword, string role)
        {
            // Check if user already exists
            if (_context.Users.Any(u => u.Username == username))
                return false;

            // Hash the password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public ClaimsPrincipal ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password); // Verify the password

            if (user == null || !isValid) return null;

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var identity = new ClaimsIdentity(claims, "Basic");
            return new ClaimsPrincipal(identity);
        }
    }
}

