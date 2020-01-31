using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Bangazon.Models.OrderViewModels;

namespace Bangazon.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Cart()
        {
            var user = await GetUserAsync();

            var order = await _context.Order
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.PaymentTypeId == null);

            var totalCost = order.OrderProducts.Sum(op => op.Product.Price);

            var paymentTypes = await _context.PaymentType.Where(pt => pt.UserId == user.Id).ToListAsync();

            var paymentOptions = paymentTypes.Select(pt => new SelectListItem
            {
                Value = pt.PaymentTypeId.ToString(),
                Text = pt.Description
            }).ToList();

            var viewModel = new ShoppingCartViewModel
            {
                TotalCost = totalCost,
                User = user,
                PaymentOptions = paymentOptions,
                SelectedPaymentId = paymentTypes.FirstOrDefault().PaymentTypeId,
                OrderDetails = new OrderDetailViewModel()
                {
                    Order = order,
                    LineItems = order.OrderProducts
                        .GroupBy(op => op.ProductId)
                        .Select(group => new OrderLineItem
                        {
                            Units = group.Count(),
                            Product = group.FirstOrDefault().Product,
                            Cost = group.Sum(op => op.Product.Price)
                        })
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Cart([Bind("SelectedPaymentId")]ShoppingCartViewModel vm)
        {
            return View();
        }

        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
