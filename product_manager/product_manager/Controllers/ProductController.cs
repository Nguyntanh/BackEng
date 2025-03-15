using Microsoft.AspNetCore.Mvc;
using product_manager.Data;
using product_manager.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace product_manager.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // List all products
        public async Task<IActionResult> Index(string searchTerm, string searchBy)
        {
            var products = from p in _context.Products
                           select p;

            // Kiểm tra nếu có giá trị tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchBy))
            {
                // Kiểm tra theo cột tìm kiếm và xác thực giá trị nhập vào
                switch (searchBy)
                {
                    case "ProductID":
                        // Kiểm tra nếu searchTerm có phải là một số nguyên hợp lệ
                        if (int.TryParse(searchTerm, out int productId))
                        {
                            products = products.Where(p => p.ProductID == productId);
                        }
                        else
                        {
                            // Thông báo lỗi nếu ProductID không hợp lệ
                            ViewData["SearchTermError"] = "Invalid ProductID. Please enter a valid integer.";
                            return View(await products.ToListAsync());
                        }
                        break;

                    case "Name":
                        // Không cần kiểm tra nếu Name là chuỗi, chỉ cần tìm kiếm theo tên
                        products = products.Where(p => p.Name.Contains(searchTerm));
                        break;

                    case "Price":
                        // Kiểm tra nếu searchTerm có phải là một số thập phân hợp lệ
                        if (decimal.TryParse(searchTerm, out decimal price))
                        {
                            products = products.Where(p => p.Price == price);
                        }
                        else
                        {
                            // Thông báo lỗi nếu Price không hợp lệ
                            ViewData["SearchTermError"] = "Invalid price value. Please enter a valid number.";
                            return View(await products.ToListAsync());
                        }
                        break;

                    case "Category":
                        // Tìm kiếm theo Category (chuỗi)
                        products = products.Where(p => p.Category.Contains(searchTerm));
                        break;

                    default:
                        break;
                }
            }

            // Pass search term and selected attribute to the view
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SearchBy"] = searchBy;

            // Trả về kết quả tìm kiếm hoặc danh sách sản phẩm nếu không có lỗi
            return View(await products.ToListAsync());
        }

        // Create a product
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Edit a product
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Delete a product
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Product Details
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Edit Product (for update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();  // Ensure that the product id matches the one being edited
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect back to the product list after update
            }

            return View(product); // Return the same view with validation errors if any
        }

        // Helper method to check if the product exists
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

    }
}
