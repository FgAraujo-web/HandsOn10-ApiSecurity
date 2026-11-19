namespace HandsOn10.Api.Models;
public sealed class AppUser { public int Id { get; set; } public string Username { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = ""; }
public sealed class AccessRequest { public int Id { get; set; } public int OwnerId { get; set; } public string Dataset { get; set; } = ""; public string Reason { get; set; } = ""; public string Status { get; set; } = "Pendente"; }
