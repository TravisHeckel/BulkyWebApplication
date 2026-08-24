using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    // ": Repository<Category>" inherits all the generic CRUD read/add/remove logic,
    // ", ICategoryRepository" adds the Update method specific to Category.
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        // ": base(db)" passes the DbContext up to the generic Repository<Category> so its
        // dbSet is wired up; we also keep our own _db reference for the Update below.
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // Stage an UPDATE of the whole Category row. Actual save happens in UnitOfWork.Save().
        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
