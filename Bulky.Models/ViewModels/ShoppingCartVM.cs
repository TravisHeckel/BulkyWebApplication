using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    // ViewModel for the cart page: the list of cart lines plus the grand total.
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        // Sum of (unit price x quantity) across every line, computed in the controller.
        public double OrderTotal { get; set; }
    }
}
