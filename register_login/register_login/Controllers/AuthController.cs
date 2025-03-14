using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using register_login.Data;
using register_login.Models;

namespace register_login.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher;

        public AuthController(AppDbContext db, PasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        // GET: /Auth/Register
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _hasher.HashPassword(null, request.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: /Auth/Login
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return NotFound("User not found.");

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid credentials.");

            return RedirectToAction("Index", "Home");
        }
    }
}
