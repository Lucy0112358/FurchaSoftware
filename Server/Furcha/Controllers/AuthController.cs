using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly AuthenticationService authenticationService;
        public AuthController(AuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = GetClaimValue("AdminId"); 
            var email = GetClaimValue("email");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID claim is missing");
            }

            return Ok(new { UserId = userId, Email = email });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public LoginResult Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            var authUser = authenticationService.LoginToGetJwtToken(authenticateRequest);

            return authUser;
        }
    }
}
