using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // Product's Update is smarter than a plain _db.Products.Update(obj). Instead of
        // overwriting the whole row, it loads the existing record and copies fields onto it.
        // Why? So a new upload can change the image, but if NO new file was chosen we DON'T
        // wipe the existing ImageUrl - we simply leave it untouched (see the "if" at the end).
        public void Update(Product obj)
        {
            //_db.Products.Update(obj);
            var objFromDb = _db.Products.FirstOrDefault(u=> u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                // Only overwrite the image path when a new image was actually uploaded.
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
