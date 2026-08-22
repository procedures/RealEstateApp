using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models;

public class AgentAdminItem
{
    public int AgentId { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string Email { get; set; } = "";
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string Role { get; set; } = "Agent";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int ClosedDeals { get; set; }

    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public int TotalCount { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoPublicId { get; set; }
}

// Үүсгэх / засах маягт
public class AgentInput
{
    public int AgentId { get; set; }

    [Required(ErrorMessage = "Овог оруулна уу")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Нэр оруулна уу")]
    public string FirstName { get; set; } = "";

    public string? Phone { get; set; }

    [Required(ErrorMessage = "И-мэйл оруулна уу")]
    [EmailAddress(ErrorMessage = "И-мэйл буруу форматтай")]
    public string Email { get; set; } = "";

    public int? BranchId { get; set; }
    public string Role { get; set; } = "Agent";

    // Зөвхөн шинээр үүсгэх үед
    public string? Password { get; set; }
}

public class BranchAdminItem
{
    public int BranchId { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }
    public int AgentCount { get; set; }
}