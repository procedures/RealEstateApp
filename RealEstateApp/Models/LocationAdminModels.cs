namespace RealEstateApp.Models;

/// <summary>Админ урьдчилан бэлдсэн "Байршил" утга (dbo.Locations).</summary>
public class LocationMasterItem
{
    public int LocationId { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    /// <summary>Одоогоор хэдэн бүртгэлтэй хөрөнгө энэ утгыг ашиглаж байгаа (dbo.Properties-оос тоолсон).</summary>
    public int UsageCount { get; set; }
}

/// <summary>Админ урьдчилан бэлдсэн "Хотхон" утга (dbo.Complexes).</summary>
public class ComplexMasterItem
{
    public int ComplexId { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int UsageCount { get; set; }
}
