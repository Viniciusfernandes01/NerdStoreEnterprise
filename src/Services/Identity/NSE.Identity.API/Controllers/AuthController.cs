using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens;
using NSE.Identity.API.Models;
using NSE.Identity.API.Extensions;

namespace NSE.Identity.API.Controllers
{
  [ApiController]
  [Route("/api/identity")]
  public class AuthController : Controller
  {

    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppSettings _appSettings;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppSettings> appSettings
    )
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _appSettings = appSettings.Value;
    }
    [HttpPost("New account")]
    public async Task<ActionResult> Register(UserRegister userRegister)
    {
      if (!ModelState.IsValid) return BadRequest();

      var user = new IdentityUser
      {
        UserName = userRegister.Email,
        Email = userRegister.Email,
        EmailConfirmed = true
      };

      var result = await _userManager.CreateAsync(user, userRegister.Password);

      if (result.Succeeded)
      {
        await _signInManager.SignInAsync(user, false);
        return Ok(await GenerateJwt(userRegister.Email));
      }

      return BadRequest();
    }

    [HttpPost("Authentication")]
    public async Task<ActionResult> Login(UserLogin userLogin)
    {
      if (!ModelState.IsValid) return BadRequest();

      var result = await _signInManager.PasswordSignInAsync(userLogin.Email,
      userLogin.Password,
      false, true);

      if (result.Succeeded)
      {
        return Ok(await GenerateJwt(userLogin.Email));
      }

      return BadRequest();

    }

    private async Task<UserResponseLogin> GenerateJwt(string email)
    {
      var user =  await _userManager.FindByEmailAsync(email);
      var claims = await _userManager.GetClaimsAsync(user);
      var userRoles = await _userManager.GetRolesAsync(user);

      claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
      claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
      claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
      claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
      claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
      
      foreach (var userRole in userRoles)
      {
        claims.Add(new Claim("role", userRole));
      }

      var identityClaims =  new ClaimsIdentity();
      identityClaims.AddClaims(claims);

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

      var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = _appSettings.Emitter,
        Audience = _appSettings.ValidIn,
        Subject = identityClaims,
        Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiresHours),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      });

      var encodedToken = tokenHandler.WriteToken(token);

      var response = new UserResponseLogin
      {
        AccessToken = encodedToken,
        ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiresHours).TotalSeconds,
        UserToken = new UserToken
        {
          Id = user.Id,
          Email = user.Email,
          Claims = claims.Select(c => new UserClaim {Type = c.Type, Value = c.Value})
        }
      };

      return response;
    }

    private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(
        1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
  }
}