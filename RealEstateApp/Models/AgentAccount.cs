namespace RealEstateApp.Models;

public class AgentAccount
{
    public int AgentId { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? Phone { get; set; }
    public string Email { get; set; } = "";
    public int? BranchId { get; set; }
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Agent";
    public bool IsActive { get; set; }

    public string FullName => $"{LastName} {FirstName}".Trim();
}

// Процедурын @ResponseCode / @ResponseMessage-ийг хүлээж авах
public record ProcResult(int Code, string Message)
{
    public bool Ok => Code == 0;
}