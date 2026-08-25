using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // [ValidateNever]
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;            // [Range]
using System.ComponentModel.DataAnnotations.Schema;     // [ForeignKey], [NotMapped]
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    // One row = one product a specific user has placed in their cart, with a quantity.
    public class ShoppingCart
    {
        public int Id { get; set; }

        // FK column -> which product this cart line is for.
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }   // navigation property (loaded via Include)

        [Range(1,1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        // FK to the Identity user who owns this cart line. It's a string because
        // IdentityUser's primary key is a GUID stored as text.
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        // [NotMapped] -> this is a calculated value used only at runtime for display.
        // EF Core will NOT create a "Price" column for it. It gets set in the CartController
        // based on quantity (tiered pricing), then used to compute the order total.
        [NotMapped]
        public double Price { get; set; }
    }
}
