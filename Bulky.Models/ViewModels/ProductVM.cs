using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // [ValidateNever]
using Microsoft.AspNetCore.Mvc.Rendering;               // SelectListItem (dropdown items)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    // A "ViewModel" (VM) bundles together everything a single view/page needs.
    // The Product edit form needs TWO things: the product itself, AND the list of
    // categories to fill the category dropdown. Rather than pass them separately,
    // we wrap them in one object and hand that to the view.
    public class ProductVM
    {
        // The product being created or edited (bound to the form fields).
        public Product Product { get; set; }

        // The <option> items for the Category dropdown. It is populated in the controller,
        // not submitted by the form, so [ValidateNever] keeps it out of form validation.
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList {  get; set; }
    }
}
