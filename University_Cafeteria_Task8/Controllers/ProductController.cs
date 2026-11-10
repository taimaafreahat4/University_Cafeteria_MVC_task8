
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using University_Cafeteria_Task8.Data;
using University_Cafeteria_Task8.Models;

namespace University_Cafeteria_Task8.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all products
        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();

            return View(products);
        }

        // Display create page
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // Save new product

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) ||
                product.Price < 0 ||
                product.Quantity < 0)
            {
                return BadRequest("Invalid product information.");
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Display edit page
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Save updated product

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Edit(Product product)
        {
            var oldProduct = _context.Products.Find(product.Id);

            if (oldProduct == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(product.Name) ||
                product.Price < 0 ||
                product.Quantity < 0)
            {
                return BadRequest("Invalid product information.");
            }

            oldProduct.Name = product.Name;
            oldProduct.Description = product.Description;
            oldProduct.Price = product.Price;
            oldProduct.Quantity = product.Quantity;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Display product details
        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Delete product
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            bool usedInRequests = _context.RequestItems
                .Any(x => x.ProductId == id);

            if (usedInRequests)
            {
                TempData["Error"] =
                    "Cannot delete a product used in supply requests.";

                return RedirectToAction("Index");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
