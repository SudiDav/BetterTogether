using BetterTogether.Data;
using BetterTogether.Models;
using BetterTogether.Models.ViewModels;
using BetterTogether.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BetterTogether.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;// To allow me to save the image to the server

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }
        public ProductsController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel
            {
                ProductTypes = _context.ProductTypes.ToList(),
                SpecialTags = _context.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }        

        // Get the lists of all products and there tags and types
        public async Task<IActionResult> Index()
        {
            var products =  _context.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        //Get the products created
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        //Post : Products Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }

            _context.Products.Add(ProductsVM.Products);
            await _context.SaveChangesAsync();

            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _context.Products.Find(ProductsVM.Products.Id);

            if (files.Count != 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.ImageUrl = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                productsFromDb.ImageUrl = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }

        //Edit Get method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _context.Products.Include(x => x.SpecialTags)
                                                         .Include(x => x.ProductTypes)
                                                         .SingleOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        //post method for edit action4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _context.Products.Where(x => x.Id == ProductsVM.Products.Id).FirstOrDefault();

                // if the user is updating a new image
                if (files.Count > 0 && files[0]!=null)
                {
                    //Get the path of the image
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                    var new_extension = Path.GetExtension(files[0].FileName);
                    var old_extension = Path.GetExtension(productFromDb.ImageUrl);

                    if (System.IO.File.Exists(Path.Combine(uploads,ProductsVM.Products.Id + old_extension)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + old_extension));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + new_extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    ProductsVM.Products.ImageUrl = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + new_extension;
                }

                if (ProductsVM.Products.ImageUrl !=null)
                {
                    productFromDb.ImageUrl = ProductsVM.Products.ImageUrl;
                }

                productFromDb.Name = ProductsVM.Products.Name;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.SpecialTagId = ProductsVM.Products.SpecialTagId;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ProductsVM);
        }

        //Detail Get method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _context.Products.Include(x => x.SpecialTags)
                                                         .Include(x => x.ProductTypes)
                                                         .SingleOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        //Delete Get method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _context.Products.Include(x => x.SpecialTags)
                                                         .Include(x => x.ProductTypes)
                                                         .SingleOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }
        //POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                var ext = Path.GetExtension(products.ImageUrl);

                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + ext)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + ext));
                }
                _context.Products.Remove(products);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}