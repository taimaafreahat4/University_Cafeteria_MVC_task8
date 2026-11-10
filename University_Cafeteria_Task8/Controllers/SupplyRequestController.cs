
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Cafeteria_Task8.Data;
using University_Cafeteria_Task8.Models;

namespace University_Cafeteria_Task8.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SupplyRequestController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // INDEX
        [HttpGet]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var requests = _context.SupplyRequests
                .Include(x => x.RequestItems)
                .ThenInclude(x => x.Product)
                .AsQueryable();

            if (!User.IsInRole("Admin") &&
                !User.IsInRole("SuperAdmin"))
            {
                requests = requests.Where(x => x.UserId == userId);
            }

            return View(requests.ToList());
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than zero.";

                return RedirectToAction("Create");
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var request = new SupplyRequest
            {
                RequestDate = DateTime.Now,
                Status = "Pending",
                UserId = userId,
                TotalPrice = product.Price * quantity
            };

            var item = new RequestItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                Price = product.Price
            };

            request.RequestItems.Add(item);

            _context.SupplyRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var request = _context.SupplyRequests
                .Include(x => x.RequestItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => x.Id == id);

            if (request == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (!User.IsInRole("Admin") &&
                !User.IsInRole("SuperAdmin") &&
                request.UserId != userId)
            {
                return Forbid();
            }

            return View(request);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var request = _context.SupplyRequests
                .Include(x => x.RequestItems)
                .FirstOrDefault(x => x.Id == id);

            if (request == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (request.UserId != userId)
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Only pending requests can be edited.");

            ViewBag.Products = _context.Products.ToList();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, int productId, int quantity)
        {
            var request = _context.SupplyRequests
                .Include(x => x.RequestItems)
                .FirstOrDefault(x => x.Id == id);

            if (request == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            if (request.UserId != userId)
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Only pending requests can be edited.");

            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than zero.";
                return RedirectToAction("Edit", new { id });
            }

            var product = _context.Products.Find(productId);

            if (product == null)
                return NotFound();

            var item = request.RequestItems.FirstOrDefault();

            if (item == null)
                return BadRequest("Request has no items.");

            if (request.RequestItems.Count != 1)
                return BadRequest("This form supports one product per request.");

            item.ProductId = product.Id;
            item.Quantity = quantity;
            item.Price = product.Price;

            request.TotalPrice = product.Price * quantity;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var request = _context.SupplyRequests
                .Include(x => x.RequestItems)
                .FirstOrDefault(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (request.UserId != userId)
            {
                return Forbid();
            }

            if (request.Status != "Pending")
            {
                return BadRequest("Only pending requests can be deleted.");
            }

            _context.RequestItems.RemoveRange(request.RequestItems);

            _context.SupplyRequests.Remove(request);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var request = _context.SupplyRequests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            if (request.Status != "Pending")
            {
                return BadRequest("Only pending requests can be approved.");
            }

            request.Status = "Approved";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var request = _context.SupplyRequests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            if (request.Status != "Pending")
            {
                return BadRequest("Only pending requests can be rejected.");
            }

            request.Status = "Rejected";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
