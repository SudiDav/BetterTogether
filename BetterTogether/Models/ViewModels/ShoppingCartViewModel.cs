using System.Collections.Generic;

namespace BetterTogether.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Products> Products { get; set; }
        public Appointments Appointments { get; set; }
    }
}
