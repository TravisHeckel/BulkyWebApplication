using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    // "SD" = Static Details. A central home for constants used across the whole app.
    // Defining role names ONCE here (instead of typing the string "admin" in many places)
    // prevents typos and makes renaming a role a one-line change. Used like:
    //   [Authorize(Roles = SD.Role_Admin)]
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "admin";
        public const string Role_Employee = "Employee";
    }
}
