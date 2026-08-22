namespace RealEstateApp.Models;

public class AgentPublicItem
{
    public int AgentId { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string Email { get; set; } = "";
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public DateTime CreatedAt { get; set; }

    public int ActiveListings { get; set; }
    public int ClosedDeals { get; set; }
    public int TotalListings { get; set; }
    public string? LatestImagePath { get; set; }

    public int TotalCount { get; set; }
    public string? PhotoUrl { get; set; }
}

public class AgentPublicDetail
{
    public int AgentId { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string Email { get; set; } = "";
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public DateTime CreatedAt { get; set; }

    public int ActiveListings { get; set; }
    public int ClosedDeals { get; set; }
    public int TotalListings { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? TopDistrict { get; set; }
    public string? PhotoUrl { get; set; }
}