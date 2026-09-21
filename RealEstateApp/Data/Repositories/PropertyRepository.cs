using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IPropertyRepository
{
    Task<PagedResult<PropertyListItem>> ListAsync(PropertyFilter filter);
    Task<PropertyDetailDto?> GetByIdAsync(int id);
    Task<PagedResult<PropertyListItem>> ListByAgentAsync(int agentId, int? status, int pageNumber, int pageSize);
    Task<(ProcResult Result, int NewPropertyId)> InsertAsync(PropertyInput input, int agentId);
    Task<ProcResult> UpdateAsync(int propertyId, PropertyInput input, int actorAgentId);
    Task<ProcResult> PublishAsync(int propertyId, int adminAgentId);
    Task<ProcResult> UnpublishAsync(int propertyId, int adminAgentId);
    Task<ProcResult> RejectAsync(int propertyId, int adminAgentId, string? note);
    Task<(ProcResult Result, List<string> PublicIds)> DeleteAsync(int propertyId, int actorAgentId);
    Task<PagedResult<PropertyListItem>> ListByAgentPublicAsync(int agentId, string sortBy, int pageNumber, int pageSize);
    Task<string?> GetTitleAsync(int propertyId);
    Task<ProcResult> SetTitleAsync(int propertyId, string? title, int actorAgentId);
}

public class PropertyRepository : IPropertyRepository
{
    private readonly IDbConnectionFactory _factory;
    public PropertyRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<PagedResult<PropertyListItem>> ListAsync(PropertyFilter f)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@OnlyPublished", f.OnlyPublished);
        p.Add("@Status", f.Status);
        p.Add("@DistrictId", f.DistrictId);
        p.Add("@PropertyTypeId", f.PropertyTypeId);
        p.Add("@TransactionTypeId", f.TransactionTypeId);
        p.Add("@MinPrice", f.MinPrice);
        p.Add("@MaxPrice", f.MaxPrice);
        p.Add("@RoomCount", f.RoomCount);
        p.Add("@Search", f.Search);
        p.Add("@PageNumber", f.PageNumber);
        p.Add("@PageSize", f.PageSize);
        p.Add("@MinArea", f.MinArea);
        p.Add("@MaxArea", f.MaxArea);
        p.Add("@HasGarage", f.HasGarage);
        p.Add("@SortBy", f.SortBy);

        var rows = (await db.QueryAsync<PropertyListItem>(
            "dbo.usp_Property_List", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<PropertyListItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<PropertyDetailDto?> GetByIdAsync(int id)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", id);

        return await db.QueryFirstOrDefaultAsync<PropertyDetailDto>(
            "dbo.usp_Property_GetById", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedResult<PropertyListItem>> ListByAgentAsync(
    int agentId, int? status, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@Status", status);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<PropertyListItem>(
            "dbo.usp_Property_ListByAgent", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<PropertyListItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<(ProcResult, int)> InsertAsync(PropertyInput i, int agentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ContractNumber", i.ContractNumber);
        p.Add("@ContractDate", i.ContractDate);
        p.Add("@TransactionTypeId", i.TransactionTypeId);
        p.Add("@PropertyTypeId", i.PropertyTypeId);
        p.Add("@DistrictId", i.DistrictId);
        p.Add("@LocationDescription", i.LocationDescription);
        p.Add("@ComplexName", i.ComplexName);
        p.Add("@Address", i.Address);
        p.Add("@Price", i.Price);
        p.Add("@AreaSize", i.AreaSize);
        p.Add("@RoomCount", i.RoomCount);
        p.Add("@CommissionedYear", i.CommissionedYear);
        p.Add("@BuildingFloors", i.BuildingFloors);
        p.Add("@FloorNumber", i.FloorNumber);
        p.Add("@WindowDirectionId", i.WindowDirectionId);
        p.Add("@HasGarage", i.HasGarage);
        p.Add("@ContactPhone", i.ContactPhone);
        p.Add("@OwnerSelfSelling", i.OwnerSelfSelling);
        p.Add("@AgentId", agentId);
        p.Add("@NewPropertyId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Property_Insert", p, commandType: CommandType.StoredProcedure);

        var result = new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
        return (result, p.Get<int>("@NewPropertyId"));
    }

    public async Task<ProcResult> UpdateAsync(int propertyId, PropertyInput i, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@ContractNumber", i.ContractNumber);
        p.Add("@ContractDate", i.ContractDate);
        p.Add("@TransactionTypeId", i.TransactionTypeId);
        p.Add("@PropertyTypeId", i.PropertyTypeId);
        p.Add("@DistrictId", i.DistrictId);
        p.Add("@LocationDescription", i.LocationDescription);
        p.Add("@ComplexName", i.ComplexName);
        p.Add("@Address", i.Address);
        p.Add("@Price", i.Price);
        p.Add("@AreaSize", i.AreaSize);
        p.Add("@RoomCount", i.RoomCount);
        p.Add("@CommissionedYear", i.CommissionedYear);
        p.Add("@BuildingFloors", i.BuildingFloors);
        p.Add("@FloorNumber", i.FloorNumber);
        p.Add("@WindowDirectionId", i.WindowDirectionId);
        p.Add("@HasGarage", i.HasGarage);
        p.Add("@ContactPhone", i.ContactPhone);
        p.Add("@OwnerSelfSelling", i.OwnerSelfSelling);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Property_Update", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> PublishAsync(int propertyId, int adminAgentId)
    => await AdminActionAsync("dbo.usp_Property_Publish", propertyId, adminAgentId, null);

    public async Task<ProcResult> UnpublishAsync(int propertyId, int adminAgentId)
        => await AdminActionAsync("dbo.usp_Property_Unpublish", propertyId, adminAgentId, null);

    public async Task<ProcResult> RejectAsync(int propertyId, int adminAgentId, string? note)
        => await AdminActionAsync("dbo.usp_Property_Reject", propertyId, adminAgentId, note);

    // Гурван процедурын параметр ижил тул нэг туслах метод
    private async Task<ProcResult> AdminActionAsync(string proc, int propertyId, int adminAgentId, string? note)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@AdminAgentId", adminAgentId);
        if (note is not null) p.Add("@Note", note);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync(proc, p, commandType: CommandType.StoredProcedure);

        return new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<(ProcResult, List<string>)> DeleteAsync(int propertyId, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@DeletedImages", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        // Процедур нь Cloudinary PublicId-үүдийг мөрөөр буцаана
        var publicIds = (await db.QueryAsync<string>(
            "dbo.usp_Property_Delete", p, commandType: CommandType.StoredProcedure))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");

        return (result, publicIds);
    }
    public async Task<PagedResult<PropertyListItem>> ListByAgentPublicAsync(
    int agentId, string sortBy, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@SortBy", sortBy);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<PropertyListItem>(
            "dbo.usp_Property_ListByAgentPublic", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<PropertyListItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<string?> GetTitleAsync(int propertyId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);

        return await db.QueryFirstOrDefaultAsync<string?>(
            "dbo.usp_Property_GetTitle", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<ProcResult> SetTitleAsync(int propertyId, string? title, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@Title", title);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Property_SetTitle", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
    }
}