namespace mange_tasks.Server.Models;

public record RegisterRequest(string Email, string UserName, string Password, string FullName);
public record LoginRequest(string Identifier, string Password); // Identifier can be Email or UserName
public record AuthResponse(string Token);
