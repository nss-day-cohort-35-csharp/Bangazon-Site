using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.OrderViewModels
{
    public class ShoppingCartViewModel
    {
        public OrderDetailViewModel OrderDetails { get; set; }
        public double TotalCost { get; set; }
        public List<SelectListItem> PaymentOptions { get; set; }
        public int SelectedPaymentId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
