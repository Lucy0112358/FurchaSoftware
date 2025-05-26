using Domain.Attributes;

namespace FurchaAdminApi.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IPermissionService permissionService)
        {
            var endpoint = context.GetEndpoint();
            var permissionAttributes = endpoint?
                .Metadata?
                .GetOrderedMetadata<RequiresPermissionAttribute>();

            if (permissionAttributes != null && permissionAttributes.Any())
            {
                var adminIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "AdminId");
                if (adminIdClaim == null || !int.TryParse(adminIdClaim.Value, out var adminId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Admin ID not found.");
                    return;
                }

                var permissions = await permissionService.GetPermissionsByAdminId(adminId);

                foreach (var required in permissionAttributes)
                {
                    if (!permissions.Any(p => p.Name == required.PermissionName))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync($"Missing permission: {required.PermissionName}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
