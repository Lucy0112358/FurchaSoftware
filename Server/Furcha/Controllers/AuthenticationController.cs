using FurchaAdminApi.Models;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService authenticationService;
        public AuthenticationController(AuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost]
        public string Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            var token = authenticationService.LoginToGetJwtToken(authenticateRequest);

            return token;
        }
    }
}
