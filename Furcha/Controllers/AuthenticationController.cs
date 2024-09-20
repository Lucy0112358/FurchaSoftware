using FurchaAdminApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    public class AuthenticationController : Controller
    {

        [HttpPost]
        [AllowAnonymous]
        public string Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            var token = authenticationService.LoginToGetJwtToken(authenticateRequest);

            return token;
        }
    }
}
