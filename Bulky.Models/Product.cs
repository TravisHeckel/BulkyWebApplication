using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;          // [Key], [Required], [Range], [Display]
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;   // [ForeignKey], [NotMapped]
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // [ValidateNever]

namespace Bulky.Models
{
    // The Product entity -> "Products" table. This is the main item the store sells (books).
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        // No [Required], so this column is optional/nullable.
        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        // The store has tiered pricing. [Display(Name=...)] sets the form label,
        // [Range] restricts the accepted value both on the form and during validation.
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1,1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        // ---- Relationship to Category (many Products belong to one Category) ----
        // CategoryId is the actual foreign-key COLUMN stored in the Products table.
        public int CategoryId { get; set; }

        // [ForeignKey] tells EF that the Category navigation property below is joined
        // through the CategoryId column above. This "navigation property" lets you write
        // product.Category.Name once EF has loaded (Include'd) the related row.
        [ForeignKey("CategoryId")]
        // [ValidateNever] -> do NOT validate this on form post. The user only submits a
        // CategoryId from a dropdown; the full Category object is filled in by EF, not the form,
        // so without this the model would wrongly fail validation as "Category is required".
        [ValidateNever]
        public Category Category { get; set; }

        // Path to the uploaded product image (e.g. \images\product\xxxx.png). Set in code,
        // never typed by the user, so [ValidateNever] keeps it out of form validation.
        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
