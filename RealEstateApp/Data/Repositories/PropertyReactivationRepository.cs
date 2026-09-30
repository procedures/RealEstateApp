using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IPropertyReactivationRepository
{
    Task<ProcResult> RequestAsync(int propertyId, ReactivationInput input, int agentId);
    Task<List<PendingReactivation>> ListPendingAsync();
    Task<ProcResult> ApproveAsync(int requestId, int adminAgentId, string? note);
    Task<ProcResult> RejectAsync(int requestId, int adminAgentId, string? note);
}

public class PropertyReactivationRepository : IPropertyReactivationRepository
{
    private readonly IDbConnectionFactory _factory;
    public PropertyReactivationRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ProcResult> RequestAsync(int propertyId, ReactivationInput i, int agentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@Reason", i.Reason);
        p.Add("@NewContractEndDate", i.NewContractEndDate);
        p.Add("@AgentId", agentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyReactivation_Request", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<List<PendingReactivation>> ListPendingAsync()
    {
        using var db = _factory.Create();
        var rows = await db.QueryAsync<PendingReactivation>(
            "dbo.usp_PropertyReactivation_ListPending", commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> ApproveAsync(int requestId, int adminAgentId, string? note)
        => await DecideAsync("dbo.usp_PropertyReactivation_Approve", requestId, adminAgentId, note);

    public async Task<ProcResult> RejectAsync(int requestId, int adminAgentId, string? note)
        => await DecideAsync("dbo.usp_PropertyReactivation_Reject", requestId, adminAgentId, note);

    private async Task<ProcResult> DecideAsync(string proc, int requestId, int adminAgentId, string? note)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@RequestId", requestId);
        p.Add("@AdminAgentId", adminAgentId);
        p.Add("@AdminNote", note);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync(proc, p, commandType: CommandType.StoredProcedure);

        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }
}
