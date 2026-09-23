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

/// <summary>Агентын сонгосон сарын хувийн статистик ("Миний сарын статистик").
/// Хамтран борлуулсан хаалт (CoAgent) энд бүтэн дүнгээрээ (split хийлгүй) тооцогдоно.</summary>
public class AgentMonthlyStats
{
    public int NewListingsCount { get; set; }
    public int SoldCount { get; set; }
    public decimal SoldTotalAmount { get; set; }
    public decimal PrevMonthSoldTotalAmount { get; set; }
    public int ActivePropertiesCount { get; set; }
}

/// <summary>Админ: сонгосон жилийн сар бүрийн борлуулалтын нийт (систем даяар,
/// нэг хаалтыг нэг л удаа тоолно — хамтран борлуулсан ч давхардуулахгүй).</summary>
public class MonthlySalesPoint
{
    public int MonthNumber { get; set; }
    public int SoldCount { get; set; }
    public decimal SoldTotalAmount { get; set; }
}

/// <summary>Админ: дүүрэг эсвэл хотхоны нэрээр бүртгэл/борлуулалтын задаргаа.</summary>
public class LocationBreakdownRow
{
    public string Name { get; set; } = "";
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public decimal? AvgPrice { get; set; }
    public decimal? AvgPricePerSqm { get; set; }
    public int SoldCount { get; set; }
    public decimal SoldTotalAmount { get; set; }
}

public class LocationBreakdownResult
{
    public List<LocationBreakdownRow> Districts { get; set; } = new();
    public List<LocationBreakdownRow> Complexes { get; set; } = new();
}

/// <summary>Админ: сонгосон жилийн сар бүрийн хөрөнгийн урсгал
/// (шинэ бүртгэл → нийтлэгдсэн → хаагдсан).</summary>
public class PropertyFunnelPoint
{
    public int MonthNumber { get; set; }
    public int NewCount { get; set; }
    public int PublishedCount { get; set; }
    public int ClosedCount { get; set; }
}

public class PropertyFunnelResult
{
    public List<PropertyFunnelPoint> Months { get; set; } = new();
    /// <summary>Цуцлагдсан нийт тоо (огноогүй тул сараар биш, нийт дүнгээр).</summary>
    public int CancelledTotal { get; set; }
}
