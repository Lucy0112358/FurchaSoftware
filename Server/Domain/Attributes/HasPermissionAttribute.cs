using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Domain.Attributes
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionEnum permission) : base(policy: permission.ToString())
        {

        }
    }
}