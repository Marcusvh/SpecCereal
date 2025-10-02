using Cereal.Interfaces;
using CerealLib.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cereal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthManager authManager;
        public UserController(IAuthManager auth)
        {
            authManager = auth;
        }
        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] LoginCredentialsDTO loginCredentials)
        {
            authManager.CreateUser(loginCredentials.Username, loginCredentials.Password);
        }
    }
}
