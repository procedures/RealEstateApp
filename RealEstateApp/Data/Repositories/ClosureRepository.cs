using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IClosureRepository
{
    Task<ProcResult> RequestAsync(int propertyId, ClosureInput input, int agentId);
    Task<List<PropertyClosureDto>> GetByPropertyAsync(int propertyId);
    Task<List<PendingClosure>> ListPendingAsync();
    Task<ProcResult> ApproveAsync(int closureId, int adminAgentId, string? note);
    Task<ProcResult> RejectAsync(int closureId, int adminAgentId, string? note);
    Task<ClosureDetailDto?> GetByIdAsync(int closureId);
    Task<ProcResult> SetExtraAsync(int closureId, ClosureExtraInfo info, int actorAgentId);
    Task<ClosureExtraInfo?> GetExtraAsync(int closureId);
}

public class ClosureRepository : IClosureRepository
{
    private readonly IDbConnectionFactory _factory;
    public ClosureRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ProcResult> RequestAsync(int propertyId, ClosureInput i, int agentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@ClosureReasonId", i.ClosureReasonId);
        p.Add("@ClosureDate", i.ClosureDate);
        p.Add("@SoldTotalPrice", i.SoldTotalPrice);
        p.Add("@SoldPricePerSqm", i.SoldPricePerSqm);
        p.Add("@BuyerLastName", i.BuyerLastName);
        p.Add("@BuyerFirstName", i.BuyerFirstName);
        p.Add("@BuyerRegNo", i.BuyerRegNo);
        p.Add("@BuyerPhone", i.BuyerPhone);
        p.Add("@CoAgentBranchId", i.CoAgentBranchId);
        p.Add("@CoAgentLastName", i.CoAgentLastName);
        p.Add("@CoAgentFirstName", i.CoAgentFirstName);
        p.Add("@CoAgentPhone", i.CoAgentPhone);
        p.Add("@AgentId", agentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyClosure_Request", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<List<PropertyClosureDto>> GetByPropertyAsync(int propertyId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);

        var rows = await db.QueryAsync<PropertyClosureDto>(
            "dbo.usp_PropertyClosure_GetByProperty", p, commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<List<PendingClosure>> ListPendingAsync()
    {
        using var db = _factory.Create();
        var rows = await db.QueryAsync<PendingClosure>(
            "dbo.usp_PropertyClosure_ListPending", commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> ApproveAsync(int closureId, int adminAgentId, string? note)
        => await DecideAsync("dbo.usp_PropertyClosure_Approve", closureId, adminAgentId, note);

    public async Task<ProcResult> RejectAsync(int closureId, int adminAgentId, string? note)
        => await DecideAsync("dbo.usp_PropertyClosure_Reject", closureId, adminAgentId, note);

    private async Task<ProcResult> DecideAsync(string proc, int closureId, int adminAgentId, string? note)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ClosureId", closureId);
        p.Add("@AdminAgentId", adminAgentId);
        p.Add("@AdminNote", note);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync(proc, p, commandType: CommandType.StoredProcedure);

        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }
    public async Task<ClosureDetailDto?> GetByIdAsync(int closureId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ClosureId", closureId);

        return await db.QueryFirstOrDefaultAsync<ClosureDetailDto>(
            "dbo.usp_PropertyClosure_GetById", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<ProcResult> SetExtraAsync(int closureId, ClosureExtraInfo info, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ClosureId", closureId);
        p.Add("@PaymentMethod", info.PaymentMethod);
        p.Add("@HasBuyerRepresentative", info.HasBuyerRepresentative);
        p.Add("@BuyerRepLastName", info.BuyerRepLastName);
        p.Add("@BuyerRepFirstName", info.BuyerRepFirstName);
        p.Add("@BuyerRepRegNo", info.BuyerRepRegNo);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyClosure_SetExtra", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ClosureExtraInfo?> GetExtraAsync(int closureId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ClosureId", closureId);

        return await db.QueryFirstOrDefaultAsync<ClosureExtraInfo>(
            "dbo.usp_PropertyClosure_GetExtra", p, commandType: CommandType.StoredProcedure);
    }
}