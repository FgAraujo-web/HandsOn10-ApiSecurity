using HandsOn10.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HandsOn10.Api.Data;
public static class SeedData {
 public static async Task InitializeAsync(AppDbContext db, IPasswordHasher<AppUser> hasher) {
  await db.Database.EnsureCreatedAsync();
  if (await db.Users.AnyAsync()) return;
  var users = new[] { new AppUser { Id=1,Username="ana",Role="User" },new AppUser { Id=2,Username="bruno",Role="User" },new AppUser { Id=3,Username="admin",Role="Admin" } };
  foreach (var u in users) u.PasswordHash=hasher.HashPassword(u,"Demo123!ChangeMe");
  db.Users.AddRange(users);
  db.Requests.AddRange(new AccessRequest { Id=101,OwnerId=1,Dataset="Consumos",Reason="Análise",Status="Pendente" },new AccessRequest { Id=102,OwnerId=2,Dataset="Faturação",Reason="Relatório",Status="Pendente" });
  await db.SaveChangesAsync();
 }
}
