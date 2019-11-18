using BetterTogether.Data;
using BetterTogether.Extensions;
using BetterTogether.Models;
using BetterTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetterTogether.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ShoppingCartViewModel shoppingCartViewModel { get; set; }

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
            shoppingCartViewModel = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        //Get action method for shoppingCart
        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sessionShoppingCart");

            if (shoppingCartList.Count>0)
            {
                foreach (var CartItem in shoppingCartList)
                {
                    Products product = _context.Products.Include(p => p.SpecialTags)
                                                         .Include(p => p.ProductTypes)
                                                         .Where(p => p.Id == CartItem).FirstOrDefault();
                    shoppingCartViewModel.Products.Add(product);
                }
            }

            return View(shoppingCartViewModel);
        }

        //Post method ya appointment mu shoppingcard
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sessionShoppingCart");

            shoppingCartViewModel.Appointments.AppointmentDate = shoppingCartViewModel.Appointments.AppointmentDate
                                                                   .AddHours(shoppingCartViewModel.Appointments.AppointmentTime.Hour)
                                                                   .AddMinutes(shoppingCartViewModel.Appointments.AppointmentTime.Minute);
            Appointments appointments = shoppingCartViewModel.Appointments;
            _context.Appointments.Add(appointments);
            _context.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in shoppingCartList)
            {
                var productsAppointment = new ProductsAppointment()
                {
                  AppointmentId = appointmentId,
                  ProductId = productId
                };
                _context.ProductsAppointments.Add(productsAppointment);                 
            }
            _context.SaveChanges();
            shoppingCartList = new List<int>();
            HttpContext.Session.Set("sessionShoppingCart", shoppingCartList);

            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });
        }
        
        //Remove the an Item in the shopping cart
        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sessionShoppingCart");

            if (shoppingCartList.Count>0)
            {
                if (shoppingCartList.Contains(id))
                {
                    shoppingCartList.Remove(id);
                }                
            }

            HttpContext.Session.Set("sessionShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        //Get
        public IActionResult AppointmentConfirmation(int id)
        {
            shoppingCartViewModel.Appointments = _context.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsAppointment> prodList = _context.ProductsAppointments.Where(p => p.AppointmentId == id).ToList();

            foreach (ProductsAppointment prodAptObj in prodList)
            {
                shoppingCartViewModel.Products.Add(_context.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }

            return View(shoppingCartViewModel);
        }
    }
}