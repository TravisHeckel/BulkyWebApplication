using Microsoft.AspNetCore.Identity;                    // IdentityUser base class
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // [ValidateNever]
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;            // [Required]
using System.ComponentModel.DataAnnotations.Schema;     // [ForeignKey]
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    // ASP.NET Core Identity ships a built-in IdentityUser (Id, Email, PasswordHash, etc.).
    // We inherit from it and ADD our own columns (Name, address, CompanyId). This is the
    // standard way to extend the login/user table with app-specific fields.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        // Optional address fields (nullable via "?").
        public string? StreetAddress {get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string PostalCode { get; set; }

        // Optional link to a Company. "int?" means a user may or may not belong to one.
        public int? CompanyId { get; set; }
        [ForeignKey ("CompanyId")]
        [ValidateNever]                 // filled by EF, not by a form -> skip validation
        public Company Company { get; set; }
    }
}
