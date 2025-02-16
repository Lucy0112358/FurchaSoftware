using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        // Method to get a specific claim by type
        protected string? GetClaimValue(string claimType)
        {
            return User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        // Method to get all claims
        protected IEnumerable<Claim> GetAllClaims()
        {
            return User?.Claims ?? Enumerable.Empty<Claim>();
        }
    }
}
