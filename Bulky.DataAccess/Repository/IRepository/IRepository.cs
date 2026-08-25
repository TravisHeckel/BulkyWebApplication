using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; // Expression<Func<T,bool>> = a strongly-typed "where" clause
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // A GENERIC repository contract. "<T> where T : class" means it works for ANY entity
    // type (Category, Product, Company, ...). Controllers depend on this interface instead
    // of talking to EF/DbContext directly, which keeps data-access details in one place and
    // makes the code easy to test and swap out. T stands in for one entity, e.g. Category.
    public interface IRepository<T> where T : class
    {
        // Get many rows. Both arguments are optional:
        //   filter            -> a lambda like (u => u.CategoryId == 2) to narrow results
        //   includeProperties -> comma-separated navigation names ("Category,Company")
        //                        to eager-load related tables in the same query.
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        // Get a single row that matches the filter (or null). "tracked" decides whether EF
        // watches the returned object for changes (true = you intend to update/save it).
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);

        void Add(T entity);              // stage an INSERT
        void Remove(T entity);           // stage a DELETE
        void RemoveRange(IEnumerable<T> entity); // stage many DELETEs at once
        // NOTE: there is deliberately no Save() here - saving is the Unit of Work's job,
        // and Update() lives on each specific repository because the logic differs per entity.
    }
}
