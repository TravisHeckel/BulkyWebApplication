using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // [Required]
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    // Company entity -> "Companies" table. Bulk buyers can be tied to a company account.
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // The "?" makes each string NULLABLE (optional). Reference types are non-nullable by
        // default in this project because <Nullable>enable</Nullable> is set in the .csproj,
        // so adding "?" is how you say "this field may be left blank".
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
