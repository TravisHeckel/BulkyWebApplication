// DataAnnotations give us the [Key], [Required], [MaxLength], [Range] attributes below.
// Entity Framework Core reads these attributes to shape the database table,
// and ASP.NET Core MVC reads them to validate form input on the server.
using System.ComponentModel.DataAnnotations;
// DisplayName lives here - it controls the label text shown in the UI (asp-for tag helper).
using System.ComponentModel;

namespace Bulky.Models
{
    // A plain C# class like this is called an "entity" or "POCO" (Plain Old CLR Object).
    // EF Core maps one class -> one database table, and one property -> one column.
    public class Category
    {
        // [Key] marks this as the primary key. EF also auto-increments an int named "Id".
        [Key]
        public int Id { get; set; }

        // [Required] -> NOT NULL in the database AND "this field is mandatory" on forms.
        [Required]
        // [DisplayName] -> the friendly label rendered in the view instead of "Name".
        [DisplayName("Category Name")]
        // [MaxLength(30)] -> the column becomes NVARCHAR(30) and input longer than 30 is rejected.
        [MaxLength(30)]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        // [Range] enforces min/max on both the form and (via validation) the model.
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
