using Domain.Configuration;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Npgsql;

namespace FurchaAdminApi.Repos
{
    public class AdminRepository : BaseRepository
    {

        public AdminRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
        {
        }

        /// <summary>
        /// Retrieves a Role objevt by its Id, to send the frontend whatever they need
        /// </summary>
        /// <param name="roleId">The RoleEnum value corresponding to the Role Id.</param>
        /// <returns>The Role entity if found; otherwise, null.</returns>
        public Role GetRoleById(RoleEnum roleId)
        {
            var sql = @"SELECT * 
                FROM furcha.""Roles"" 
                WHERE ""Id"" = @Id";

            var role = Query<Role>(
                 sql: sql,
                 param: new { Id = (long)roleId }
                 ).FirstOrDefault();

            return role;
        }

    }
}
