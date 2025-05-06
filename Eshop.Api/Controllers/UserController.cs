using Eshop.Api.Entities;
using Eshop.Api.Data;
using Eshop.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization; //token using

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(EshopContext context, IConfiguration configuration) : ControllerBase
{
    private readonly EshopContext context = context;
    private readonly IConfiguration configuration = configuration;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Uživatel nebyl nalezen. Pravděpodobně neplatné ID");
        }
        return Ok(user);
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        int id = int.Parse(User.FindFirst("UserId")!.Value);
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Uživatel nebyl nalezen. Pravděpodobně neplatné ID");
        }
        return Ok(user);
    }


    //REGISTER
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(User user)
    {
        if (!EmailValidator.IsValidEmail(user.Email))
        {
            return BadRequest(new
            {
                Message = "Špatný formát emailu.",
                EmailValidator.AllowedDomains,
                EmailValidator.AllowedWebs
            });
        }
        else if (await context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest("Uživatel s tímto emailem již existuje");
        }

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        Console.WriteLine($"Přijatý email: {user.Email}");

        return Created("", user); //nechci složitě hledat URL odkaz
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized("Špatný email nebo heslo");
        }

        // Načtení konfigurace JWT
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var tokenString = JwtTokenGenerator.GenerateToken(secretKey!, issuer!, audience!, user);

        return Ok(new { Token = tokenString, UserId = user.Id, user.Name, user.Role });
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("change-role")]
    public async Task<IActionResult> ChangeUserRole(ChangeRoleRequest request)
    {
        var user = await context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound("Chyba při hledání uživatele.");
        }
        else if (user.IsHardcodedAdmin)
        {
            return BadRequest("Tento uživatel je chráněný a jeho role nelze měnit.");
        }

        user.Role = request.NewRole;
        await context.SaveChangesAsync();

        return Ok($"Role uživatele {user.Name} byla změněna na {request.NewRole}.");
    }

    [Authorize]
    [HttpPut("update-user-data")]
    public async Task<IActionResult> UpdateUserData([FromBody] UpdatedUserRequest request)
    {
        var id = int.Parse(User.FindFirst("UserId")!.Value);
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Uživatel nebyl nalezen.");
        }

        user.Name = request.Name;
        if (!EmailValidator.IsValidEmail(request.Email))
        {
            return BadRequest(new
            {
                Message = "Špatný formát emailu.",
                EmailValidator.AllowedDomains,
                EmailValidator.AllowedWebs
            });

        }
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Town = request.Town;
        user.Street = request.Street;
        user.Psc = request.Psc;

        await context.SaveChangesAsync();

        // Načtení konfigurace JWT
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        //Posílání změněného tokenu
        var token = JwtTokenGenerator.GenerateToken(secretKey!, issuer!, audience!, user);

        return Ok(token);
    }

    public class ChangeRoleRequest
    {
        public int UserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }

    public class UpdatedUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Psc { get; set; } = string.Empty;
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-by-id/{id:int}")]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return BadRequest("Uzivatel nebyl nalezen.");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return Ok(new { message = "Uživatel byl odstraněn." });
    }

}
