using Microsoft.AspNetCore.Mvc;
using signup.Data;
using signup.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Account.ToListAsync();
    }

    [HttpGet("{email}")] // Sử dụng email làm định danh duy nhất
    public async Task<ActionResult<User>> GetUser(string email)
    {
        var user = await _context.Account.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        var existingUser = await _context.Account.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            return Conflict("Email already exists.");
        }

        _context.Account.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { email = user.Email }, user);
    }

    [HttpPut("{email}")]
    public async Task<IActionResult> PutUser(string email, User user)
    {
        if (email != user.Email)
        {
            return BadRequest("Email mismatch.");
        }

        var existingUser = await _context.Account.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser == null)
        {
            return NotFound("User not found.");
        }

        _context.Entry(existingUser).CurrentValues.SetValues(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Concurrency error occurred.");
        }

        return NoContent();
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        var user = await _context.Account.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return NotFound();
        }

        _context.Account.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}