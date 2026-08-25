using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // The "Unit of Work" pattern: instead of injecting five separate repositories into a
    // controller, we inject ONE object that exposes all of them plus a single Save().
    // This guarantees every repository shares the SAME DbContext, so a group of changes
    // is committed together in one Save() (one transaction).
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }

        // Commit all staged Add/Update/Remove operations to the database at once.
        void Save();
    }
}
