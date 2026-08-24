using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // Generic CRUD (from IRepository<Product>) plus a Product-specific Update.
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
    }
}
