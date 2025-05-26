using Domain.Configuration;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Npgsql;
using NuGet.Protocol.Plugins;

namespace FurchaAdminApi.Repos
{
    public class AdminRepository : BaseRepository
    {

        public AdminRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
        {
        }

        /// <summary>
        /// Retrieves a Roles objevt by its Id, to send the frontend whatever they need
        /// </summary>
        /// <param name="roleId">The RoleEnum value corresponding to the Roles Id.</param>
        /// <returns>The Roles entity if found; otherwise, null.</returns>
        public Roles GetRoleById(RoleEnum roleId)
        {
            var sql = @"SELECT * 
                FROM furcha.""Roles"" 
                WHERE ""Id"" = @Id";

            var role = Query<Roles>(
                 sql: sql,
                 param: new { Id = (long)roleId }
                 ).FirstOrDefault();

            return role;
        }

        public List<Administrator> GetAdminsByCompanyId(int companyId)
        {
            var sql = @"SELECT * FROM furcha.""Administrators"" WHERE ""CompanyId"" = @companyId";
            var admins = Query<Administrator>(sql, new { companyId }).ToList();

            return admins.ToList();
        }

        public Administrators CreateAdmin(Administrators admin)
        {
            return Insert(admin);
        }

        public List<Roles> GetRoles()
        {
            return GetAll<Roles>().ToList();
        }

        public AdminPermissions CreateAdminPermissions(AdminPermissions adminPermissions)
        {
            return Insert(adminPermissions);
        }

/*        public async Task SeedPermissions()
        {
            var permissions = new[]
            {
        new Permission { Name = "ManageUsers", Description = "Can add/edit/delete users" },
        new Permission { Name = "ViewLogs", Description = "Can view system logs" },
        new Permission { Name = "ManageLockers", Description = "Can manage lockers" }
    };

            var sql = "INSERT INTO furcha.Permissions (Name, Description) VALUES (@Name, @Description)";
            using var connection = CreateConnection();

            foreach (var permission in permissions)
            {
                await connection.ExecuteAsync(sql, permission);
            }
        }*/


    }
}
