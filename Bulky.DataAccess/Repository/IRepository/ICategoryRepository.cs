using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // Inherits every generic method (GetAll/Get/Add/Remove) from IRepository<Category>,
    // then ADDS Update - which isn't generic because update logic can differ per entity.
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
