using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IViewingRepository
{
    Task<ProcResult> RequestAsync(int propertyId, ViewingInput input);
    Task<PagedResult<ViewingItem>> ListAsync(int? agentId, int? status, int pageNumber, int pageSize);
    Task<ProcResult> SetStatusAsync(int viewingId, int status, string? agentNote, int actorAgentId);
    Task<int> NewCountAsync(int? agentId);
    Task<ViewingStats> GetStatsAsync();
    Task<List<ViewingAgentOption>> AgentOptionsAsync();
    Task<PagedResult<ViewingItem>> ListAdminAsync(int? agentId, int? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
}

public class ViewingRepository : IViewingRepository
{
    private readonly IDbConnectionFactory _factory;
    public ViewingRepository(IDbConnectionFactory factory) => _factory = factory;

    private static void AddOutputs(DynamicParameters p)
    {
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
    }

    private static ProcResult ReadResult(DynamicParameters p) =>
        new(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");

    public async Task<ProcResult> RequestAsync(int propertyId, ViewingInput i)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@FullName", i.FullName);
        p.Add("@Phone", i.Phone);
        p.Add("@Email", i.Email);
        p.Add("@PreferredDate", i.PreferredDate);
        p.Add("@Note", i.Note);
        p.Add("@NewViewingId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        AddOutputs(p);

        await db.ExecuteAsync("dbo.usp_Viewing_Request", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<PagedResult<ViewingItem>> ListAsync(int? agentId, int? status, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@Status", status);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<ViewingItem>(
            "dbo.usp_Viewing_List", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<ViewingItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<ProcResult> SetStatusAsync(int viewingId, int status, string? agentNote, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ViewingId", viewingId);
        p.Add("@Status", status);
        p.Add("@AgentNote", agentNote);
        p.Add("@ActorAgentId", actorAgentId);
        AddOutputs(p);

        await db.ExecuteAsync("dbo.usp_Viewing_SetStatus", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<int> NewCountAsync(int? agentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);

        return await db.ExecuteScalarAsync<int>(
            "dbo.usp_Viewing_NewCount", p, commandType: CommandType.StoredProcedure);
    }
    public async Task<ViewingStats> GetStatsAsync()
    {
        using var db = _factory.Create();
        return await db.QueryFirstOrDefaultAsync<ViewingStats>(
            "dbo.usp_Viewing_Stats", commandType: CommandType.StoredProcedure)
            ?? new ViewingStats();
    }

    public async Task<List<ViewingAgentOption>> AgentOptionsAsync()
    {
        using var db = _factory.Create();
        var rows = await db.QueryAsync<ViewingAgentOption>(
            "dbo.usp_Viewing_AgentOptions", commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<PagedResult<ViewingItem>> ListAdminAsync(
        int? agentId, int? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@Status", status);
        p.Add("@FromDate", fromDate);
        p.Add("@ToDate", toDate);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<ViewingItem>(
            "dbo.usp_Viewing_ListAdmin", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<ViewingItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }
}