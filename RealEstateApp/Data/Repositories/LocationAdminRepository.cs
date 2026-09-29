using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

/// <summary>Админы "Байршил / Хотхон" урьдчилан бэлдэх жагсаалтын удирдлага
/// (dbo.Locations, dbo.Complexes). Эдгээр нь хөрөнгө бүртгэх маягтын
/// Байршил/Хотхон autocomplete-д санал болгогдох боловч агентыг шинэ
/// утга чөлөөтэй бичихээс хориглохгүй — зөвхөн урьдчилан бэлдэх сан.</summary>
public interface ILocationAdminRepository
{
    Task<List<LocationMasterItem>> ListLocationsAsync(string? search = null);
    Task<ProcResult> AddLocationAsync(string name, int actorAgentId);
    Task<ProcResult> RenameLocationAsync(int locationId, string name, int actorAgentId);
    Task<ProcResult> DeleteLocationAsync(int locationId, int actorAgentId);

    Task<List<ComplexMasterItem>> ListComplexesAsync(string? search = null);
    Task<ProcResult> AddComplexAsync(string name, int actorAgentId);
    Task<ProcResult> RenameComplexAsync(int complexId, string name, int actorAgentId);
    Task<ProcResult> DeleteComplexAsync(int complexId, int actorAgentId);
}

public class LocationAdminRepository : ILocationAdminRepository
{
    private readonly IDbConnectionFactory _factory;
    public LocationAdminRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<LocationMasterItem>> ListLocationsAsync(string? search = null)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Search", search);

        var rows = await db.QueryAsync<LocationMasterItem>(
            "dbo.usp_Location_List", p, commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> AddLocationAsync(string name, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Name", name);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Location_Add", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> RenameLocationAsync(int locationId, string name, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@LocationId", locationId);
        p.Add("@Name", name);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Location_Rename", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> DeleteLocationAsync(int locationId, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@LocationId", locationId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Location_Delete", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<List<ComplexMasterItem>> ListComplexesAsync(string? search = null)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Search", search);

        var rows = await db.QueryAsync<ComplexMasterItem>(
            "dbo.usp_Complex_List", p, commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> AddComplexAsync(string name, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Name", name);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Complex_Add", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> RenameComplexAsync(int complexId, string name, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@ComplexId", complexId);
        p.Add("@Name", name);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Complex_Rename", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> DeleteComplexAsync(int complexId, int actorAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@ComplexId", complexId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Complex_Delete", p, commandType: CommandType.StoredProcedure);
        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }
}
