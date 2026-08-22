namespace RealEstateApp.Models;

public class ViewingInput
{
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public DateTime? PreferredDate { get; set; }
    public string? Note { get; set; }
}

public class ViewingItem
{
    public int ViewingId { get; set; }
    public int PropertyId { get; set; }

    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public DateTime? PreferredDate { get; set; }
    public string? Note { get; set; }
    public int Status { get; set; }
    public string? AgentNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? HandledAt { get; set; }

    public string Address { get; set; } = "";
    public string? ComplexName { get; set; }
    public decimal Price { get; set; }
    public decimal AreaSize { get; set; }
    public int? RoomCount { get; set; }
    public string DistrictName { get; set; } = "";
    public string? MainImagePath { get; set; }

    public int AgentId { get; set; }
    public string AgentName { get; set; } = "";
    public string? HandledByName { get; set; }

    public int NewCount { get; set; }
    public int TotalCount { get; set; }
}
public class ViewingStats
{
    public int TotalCount { get; set; }
    public int NewCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int DoneCount { get; set; }
    public int CancelledCount { get; set; }
    public int TodayCount { get; set; }
}

public class ViewingAgentOption
{
    public int AgentId { get; set; }
    public string FullName { get; set; } = "";
    public int TotalCount { get; set; }
    public int NewCount { get; set; }
}