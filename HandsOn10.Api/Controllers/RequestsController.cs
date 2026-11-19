using System.Security.Claims;
using HandsOn10.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HandsOn10.Api.Controllers;
[ApiController][Authorize][Route("api/pedidos")]
public sealed class RequestsController(AppDbContext db, IConfiguration config, ILogger<RequestsController> logger) : ControllerBase {
 private int? CurrentId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:null;
 [HttpGet("meus")]
 public async Task<IActionResult> Mine() => Ok(await db.Requests.AsNoTracking().Where(x=>x.OwnerId==CurrentId).Select(x=>new {x.Id,x.Dataset,x.Reason,x.Status}).ToListAsync());
 [HttpGet("{id:int}")]
 public async Task<IActionResult> Get(int id) {
  var item=await db.Requests.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
  if(item is null) return NotFound();
  if(!User.IsInRole("Admin") && item.OwnerId!=CurrentId) { logger.LogWarning("Acesso negado a pedido {RequestId}",id); return Forbid(); }
  return Ok(new {item.Id,item.Dataset,item.Reason,item.Status,item.OwnerId});
 }
 [Authorize(Policy="AdminsOnly")][HttpGet("admin/todos")]
 public async Task<IActionResult> All() => Ok(await db.Requests.AsNoTracking().Select(x=>new {x.Id,x.OwnerId,x.Dataset,x.Status}).ToListAsync());
 // LABORATÓRIO APENAS: requer Development E flag explícita; nunca publicar.
 [HttpGet("lab/vulneravel/{id:int}")]
 public async Task<IActionResult> Vulnerable(int id) {
  if(!HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() || !config.GetValue<bool>("Lab:EnableVulnerableEndpoint")) return NotFound();
  var item=await db.Requests.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
  return item is null ? NotFound() : Ok(new {item.Id,item.OwnerId,item.Dataset,item.Reason,item.Status});
 }
}
