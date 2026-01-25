namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresPermissionAttribute : Attribute
    {
        public string PermissionName { get; }

        public RequiresPermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }
    }

}
