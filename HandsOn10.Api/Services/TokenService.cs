using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HandsOn10.Api.Models;
using Microsoft.IdentityModel.Tokens;
namespace HandsOn10.Api.Services;
public sealed class TokenService(IConfiguration config) {
 public string Create(AppUser user) {
  var key=config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured");
  var claims=new[] { new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),new Claim(ClaimTypes.Name,user.Username),new Claim(ClaimTypes.Role,user.Role) };
  var token=new JwtSecurityToken(config["Jwt:Issuer"],config["Jwt:Audience"],claims,expires:DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:Minutes",45)),signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));
  return new JwtSecurityTokenHandler().WriteToken(token);
 }
}
