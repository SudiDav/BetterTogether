using System.Linq;
using System.Threading.Tasks;
using BetterTogether.Data;
using BetterTogether.Models;
using BetterTogether.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTogether.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminUser)]
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductTypesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        
        public IActionResult Index()
        {
            return View(_context.ProductTypes.ToList());
        }

        //Get Action for create method 
        public IActionResult Create()
        {
            return View();
        }
        // Get POSt action method for create product types
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }

        //Get Action for Edit method 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType==null)
            {
                return NotFound();
            }

            return View(productType);
        }
        // POSt action method for Edit product types
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProductTypes productTypes)
        {
            if (id != productTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(productTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }

        //Get Action for Details method 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        //Get Action for Delete method 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }
        // POSt action method for Delete product types
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTypes = await _context.ProductTypes.FindAsync(id);
                _context.ProductTypes.Remove(productTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));   
        }
    }
}