using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PROGETTO2703.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) != null)
            return BadRequest(new { message = "Email già registrata." });

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Nome = model.Nome,
            Cognome = model.Cognome
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // Assegna il ruolo Utente di default
        await _userManager.AddToRoleAsync(user, "Utente");

        return Ok(new { message = "Registrazione effettuata con successo!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Unauthorized(new { message = "Credenziali non valide." });

        var checkPass = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPass.Succeeded) return Unauthorized(new { message = "Credenziali non valide." });

        var tokenString = GenerateJwtToken(user);
        return Ok(new { token = tokenString });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secretKey = _configuration["Jwt:SecurityKey"];
        //Console.WriteLine($"SecurityKey from config: {secretKey}");

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException("Jwt:SecurityKey è NULL! Controlla appsettings.json");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("Nome", user.Nome ?? ""),
            new Claim("Cognome", user.Cognome ?? "")
        };

        // Aggiunge i ruoli al token
        var userRoles = _userManager.GetRolesAsync(user).Result;
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("createrole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (string.IsNullOrEmpty(roleName)) return BadRequest(new { message = "Nome ruolo non valido." });

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            return Ok(new { message = $"Ruolo '{roleName}' creato con successo!" });
        }

        return BadRequest(new { message = "Ruolo già esistente." });
    }
}

// DTOs
public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
