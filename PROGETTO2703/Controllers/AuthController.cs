using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PROGETTO2703.DTO;
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

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Email o password errati.");
        }

        bool passwordValida = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValida)
        {
            return Unauthorized("Email o password errati.");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, "Amministratore") 
    };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(50),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize(Roles = "Amministratore")]
    [HttpPost("AssegnaRuolo")]
    public async Task<IActionResult> AssegnaRuolo([FromBody] AssegnaRuoloDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound("Utente non trovato.");

        var result = await _userManager.AddToRoleAsync(user, model.Ruolo);
        if (!result.Succeeded) return BadRequest("Errore nell'assegnazione del ruolo.");

        return Ok($"Ruolo {model.Ruolo} assegnato a {model.Email}");
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

public class AssegnaRuoloDto
{
    public string Email { get; set; }
    public string Ruolo { get; set; }
}