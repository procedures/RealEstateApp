namespace RealEstateApp.Models;

public class PropertyListItem
{
    public int PropertyId { get; set; }
    public string? ContractNumber { get; set; }
    public DateTime? ContractDate { get; set; }
    public string TransactionTypeName { get; set; } = "";
    public string PropertyTypeName { get; set; } = "";
    public string DistrictName { get; set; } = "";
    public string DistrictCode { get; set; } = "";
    public string? ComplexName { get; set; }
    public string Address { get; set; } = "";
    public decimal Price { get; set; }
    public decimal AreaSize { get; set; }
    public decimal? PricePerSqm { get; set; }
    public int? RoomCount { get; set; }
    public int? BuildingFloors { get; set; }
    public int? FloorNumber { get; set; }
    public bool HasGarage { get; set; }
    public int Status { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ContractClosedDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string AgentName { get; set; } = "";
    public int? AgentId { get; set; }
    public string? AgentPhotoUrl { get; set; }
    public string? Title { get; set; }

    // Зураг
    public string? MainImagePath { get; set; }
    public int ImageCount { get; set; }

    // usp_Property_List-ийн COUNT(*) OVER()
    public int TotalCount { get; set; }
}