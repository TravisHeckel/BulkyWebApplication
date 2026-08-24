using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // Generic CRUD (from IRepository<Company>) plus a Company-specific Update.
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company obj);
    }
}
