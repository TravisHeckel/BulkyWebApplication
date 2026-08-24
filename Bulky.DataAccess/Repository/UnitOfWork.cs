using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    // Concrete Unit of Work. It creates one instance of each repository, all sharing the
    // single ApplicationDbContext that DI hands in, and exposes Save() to commit them.
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        // Each repository is created once in the constructor and exposed as a read-only
        // property. "private set" means only this class can assign them.
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            // Every repository below is handed the SAME _db, which is what lets a single
            // Save() persist changes made across several of them together.
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
        }

        // One call flushes all pending inserts/updates/deletes to SQL in one transaction.
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
