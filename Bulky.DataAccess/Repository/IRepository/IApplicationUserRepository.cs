using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    // No extra members here - it only needs the generic read/add/remove from
    // IRepository<ApplicationUser>. It exists so users can be reached through the
    // Unit of Work like every other entity, ready for extra methods later if needed.
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {

    }
}
