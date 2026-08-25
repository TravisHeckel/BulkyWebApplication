using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore; // Include, AsNoTracking, Set<T>

namespace Bulky.DataAccess.Repository
{
    // The ONE concrete implementation of IRepository<T>, reused for every entity.
    // Writing this logic once (instead of copy-pasting GetAll/Get/Add/Remove into every
    // repository) is the whole point of the generic repository pattern.
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        // dbSet is the table for T (e.g. _db.Categories when T is Category). "internal" so
        // the entity-specific repositories in this project can reuse it in their Update().
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            // _db.Set<T>() asks EF for the DbSet matching T, so this works for any entity.
            this.dbSet = _db.Set<T>();
            //_db.Categories == dbSet
            _db.Products.Include(u => u.Category).Include(u => u.CategoryId); // (tutorial leftover; no effect)
        }

        public void Add(T entity)
        {
            dbSet.Add(entity); // marks the entity to be INSERTed on the next Save()
        }

        // Return the first row matching the filter, optionally eager-loading related tables.
        public T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                // Tracked: EF remembers this object, so later edits + Save() are persisted.
                query = dbSet;
            }
            else
            {
                // AsNoTracking: read-only and faster (EF won't watch it for changes).
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);

            // Split "Category,Company" and Include each one so navigation properties are filled.
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault(); // the matching row, or null if none
        }

        //Category, CoverType
        // Return every row matching the (optional) filter, with optional eager-loading.
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties =  null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if(!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList(); // execute the query and materialize the rows
        }

        public void Remove(T entity)
        {
           dbSet.Remove(entity);            // stage a single DELETE
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);       // stage many DELETEs
        }
    }
}
