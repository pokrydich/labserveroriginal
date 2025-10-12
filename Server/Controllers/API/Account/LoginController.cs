namespace LabServer.Server.Controllers;

using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using LabServer.Server.Models;
using LabServer.Shared.Models;

[Route("api/[controller]")]
[ApiController]
public class LoginController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly UserManager<UserModel> _userManager;
    public LoginController(IConfiguration configuration, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager)
    {
        _configuration = configuration;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ApiRequestResult<System.String>> Login([FromBody] LoginModel login)
    {
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
        if (!result.Succeeded)
        {
            return ApiRequestResult.Failure<System.String>("Username or password is invalid.");
        }
        var user = _signInManager.UserManager.Users.Single(u => u.Email == login.Email);
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
        claims.AddRange((await _userManager.GetRolesAsync(user)).Select(r => new Claim(ClaimTypes.Role, r)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

        var token = new JwtSecurityToken(
            _configuration["JwtIssuer"],
            _configuration["JwtAudience"],
            claims,
            expires: expiry,
            signingCredentials: creds
        );
        return ApiRequestResult.Success<System.String>(new JwtSecurityTokenHandler().WriteToken(token));
    }
}