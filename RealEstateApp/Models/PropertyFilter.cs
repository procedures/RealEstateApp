namespace RealEstateApp.Models;

public class PropertyFilter
{
    public bool OnlyPublished { get; set; } = true;
    public int? Status { get; set; }
    public int? DistrictId { get; set; }
    public int? PropertyTypeId { get; set; }
    public int? TransactionTypeId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinArea { get; set; }
    public decimal? MaxArea { get; set; }
    public int? RoomCount { get; set; }
    public bool? HasGarage { get; set; }
    public string? Search { get; set; }
    public int? AgentId { get; set; }
    public string? ComplexName { get; set; }
    public string? LocationDescription { get; set; }
    public string SortBy { get; set; } = "newest";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    public int Total { get; set; }
}