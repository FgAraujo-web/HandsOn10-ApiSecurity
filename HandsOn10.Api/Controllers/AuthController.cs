using HandsOn10.Api.Data;
using HandsOn10.Api.Models;
using HandsOn10.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HandsOn10.Api.Controllers;
[ApiController][Route("api/auth")]
public sealed class AuthController(AppDbContext db, IPasswordHasher<AppUser> hasher, TokenService tokens) : ControllerBase {
 public sealed record LoginInput(string Username,string Password);
 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginInput input) {
  var user=await db.Users.SingleOrDefaultAsync(x=>x.Username==input.Username);
  if(user is null || hasher.VerifyHashedPassword(user,user.PasswordHash,input.Password)==PasswordVerificationResult.Failed) return Unauthorized(new { error="Credenciais inválidas" });
  return Ok(new { access_token=tokens.Create(user),token_type="Bearer",expires_in=2700 });
 }
}
