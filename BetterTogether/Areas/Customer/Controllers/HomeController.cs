using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BetterTogether.Models;
using BetterTogether.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using BetterTogether.Extensions;

namespace BetterTogether.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var productList = await _context.Products.Include(x => x.ProductTypes)
                                                     .Include(x => x.SpecialTags)
                                                     .ToListAsync(); 
            return View(productList);
        }

        //Get action method
        public async Task<IActionResult> Details(int id)
        {

            var product = await _context.Products.Include(x => x.ProductTypes)
                                                     .Include(x => x.SpecialTags)
                                                     .Where(x => x.Id == id).FirstOrDefaultAsync();
            return View(product);
        }

        //Post action method inafanya apa kwa details
        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sessionShoppingCart");

            if (shoppingCartList == null)
            {
                shoppingCartList = new List<int>();
            }
            shoppingCartList.Add(id);
            HttpContext.Session.Set("sessionShoppingCart", shoppingCartList);

            return RedirectToAction("Index","Home", new { area = "Customer"});
        }

        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sessionShoppingCart");

            if (shoppingCartList.Count > 0)
            {
                if (shoppingCartList.Contains(id))
                {
                    shoppingCartList.Remove(id);
                }
            }
            HttpContext.Session.Set("sessionShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
