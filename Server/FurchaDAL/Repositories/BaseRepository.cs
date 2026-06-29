using System.Linq.Expressions;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace FurchaDAL.Repositories
{
    /// <summary>
    /// Generic EF Core repository providing basic CRUD over <typeparamref name="T"/>.
    /// Inherit it for entity-specific repositories (e.g. <c>SyncTaskRepository : BaseRepository&lt;SyncTask&gt;</c>),
    /// or inject <c>BaseRepository&lt;T&gt;</c> directly for simple cases.
    /// The underlying <see cref="furchaContext"/> is exposed via <see cref="Context"/> so callers can
    /// perform custom queries/transactions the same way services do today.
    /// </summary>
    /// <typeparam name="T">An entity type mapped on <see cref="furchaContext"/>.</typeparam>
    public class BaseRepository<T> where T : class
    {
        protected readonly furchaContext Db;
        protected readonly DbSet<T> DbSet;

        public BaseRepository(furchaContext db)
        {
            Db = db;
            DbSet = db.Set<T>();
        }

        /// <summary>The EF context, for custom queries/transactions beyond basic CRUD.</summary>
        public furchaContext Context => Db;

        /// <summary>Get a single entity by primary key (supports int, long, Guid, etc.). Returns null if not found.</summary>
        public virtual async Task<T?> GetAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        /// <summary>
        /// Get all rows of the entity. Use sparingly on large tables.
        /// Read-only (no change tracking) — call <see cref="Query"/> if you need to modify the results.
        /// </summary>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Get all rows matching the predicate.
        /// Read-only (no change tracking) — call <see cref="Query"/> if you need to modify the results.
        /// </summary>
        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get the first row matching the predicate, or null.
        /// Read-only (no change tracking) — call <see cref="Query"/> if you need to modify the result.
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        /// <summary>True if any row matches the predicate.</summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        /// <summary>Count rows, optionally filtered.</summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate is null
                ? await DbSet.CountAsync()
                : await DbSet.CountAsync(predicate);
        }

        /// <summary>Insert one entity and save. Returns the tracked entity (with generated keys populated).</summary>
        public virtual async Task<T> InsertAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await Db.SaveChangesAsync();
            return entity;
        }

        /// <summary>Insert many entities in one save.</summary>
        public virtual async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
            await Db.SaveChangesAsync();
        }

        /// <summary>Update an entity and save.</summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await Db.SaveChangesAsync();
            return entity;
        }

        /// <summary>Delete an entity and save.</summary>
        public virtual async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await Db.SaveChangesAsync();
        }

        /// <summary>Delete by primary key. Returns false if no row was found.</summary>
        public virtual async Task<bool> DeleteByIdAsync(object id)
        {
            var entity = await GetAsync(id);
            if (entity is null)
                return false;

            DbSet.Remove(entity);
            await Db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Composable query entry point for read scenarios that need Include/OrderBy/projection.
        /// Tracking is left on; call <c>.AsNoTracking()</c> downstream for read-only paths.
        /// </summary>
        public virtual IQueryable<T> Query()
        {
            return DbSet;
        }

        /// <summary>Persist pending changes made through <see cref="Context"/> or tracked entities.</summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }
    }
}
