namespace RealEstateApp.Models;

/// <summary>Админ самбарын чансаанд харуулах нэг агентын мөр.</summary>
public class AgentLeaderboardRow
{
    public int AgentId { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public int PropertyCount { get; set; }
    public int ClosureCount { get; set; }
    public decimal SoldTotalAmount { get; set; }

    public string FullName => $"{LastName} {FirstName}".Trim();
}

/// <summary>Тухайн агентын өөрийн хувийн статистик.</summary>
public class AgentDashboardStats
{
    public int TotalProperties { get; set; }
    public int PendingCount { get; set; }
    public int PublishedCount { get; set; }
    public int ClosureRequestCount { get; set; }
    public int ClosedCount { get; set; }
    public int ApprovedClosures { get; set; }
    public decimal SoldTotalAmount { get; set; }
    public int TotalViewings { get; set; }
    public int NewViewings { get; set; }
}
