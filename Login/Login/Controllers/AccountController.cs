using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Login.Data;
using Login.Models;
using System.Threading.Tasks;
using Login.Data;
using Microsoft.AspNetCore.Identity;

namespace Login.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                              .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                // Use PasswordHasher to verify the password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                // If the password is correct, redirect to the dashboard
                if (result == PasswordVerificationResult.Success)
                {
                    return RedirectToAction("Dashboard");
                }
            }

            // Add an error message if login fails
            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

    }
}
