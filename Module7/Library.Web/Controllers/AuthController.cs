using System.Security.Claims;
using Library.Contracts.Auth.Request;
using Library.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

[ApiController]
[Route("api")]
public sealed class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtService _jwtService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private const string AdminRole = "Admin";

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<IdentityUser> signInManager, 
        JwtService jwtService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DateOfBirth = request.BirthDay
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await CreateRoles();
        if (string.Equals(request.UserName, AdminRole, StringComparison.CurrentCultureIgnoreCase))
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return Ok(new { Message = "Пользователь успешно создан" });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return NotFound(new { Message = "Failure" });
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            false
        );

        if (!result.Succeeded)
        {
            return Unauthorized(new { Message = "Failure" });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new { access_token = token });
    }
    
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userName = User.Identity?.Name;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(new
        {
            UserName = userName,
            UserId = userId
        });
    }
    
    private async Task CreateRoles()
    {
        if (!await _roleManager.RoleExistsAsync(AdminRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(AdminRole));
        }
    }
}