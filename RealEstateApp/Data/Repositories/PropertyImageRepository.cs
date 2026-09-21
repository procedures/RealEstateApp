using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IPropertyImageRepository
{
    Task<List<PropertyImage>> ListAsync(int propertyId);
    Task<ProcResult> InsertAsync(int propertyId, string imageUrl, string publicId, int actorAgentId);
    Task<(ProcResult Result, string? DeletedPublicId)> DeleteAsync(int imageId, int actorAgentId);
    Task<ProcResult> SetMainAsync(int imageId, int actorAgentId);
    Task<ProcResult> ReorderAsync(int propertyId, IReadOnlyList<int> orderedImageIds, int actorAgentId);
}

public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly IDbConnectionFactory _factory;
    public PropertyImageRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<PropertyImage>> ListAsync(int propertyId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);

        var rows = await db.QueryAsync<PropertyImage>(
            "dbo.usp_PropertyImage_ListByProperty", p, commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<ProcResult> InsertAsync(int propertyId, string imageUrl, string publicId, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@PropertyId", propertyId);
        p.Add("@ImagePath", imageUrl);
        p.Add("@PublicId", publicId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@NewImageId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyImage_Insert", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<(ProcResult, string?)> DeleteAsync(int imageId, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ImageId", imageId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@DeletedPublicId", dbType: DbType.String, direction: ParameterDirection.Output, size: 300);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyImage_Delete", p, commandType: CommandType.StoredProcedure);

        var result = new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");

        return (result, p.Get<string?>("@DeletedPublicId"));
    }

    public async Task<ProcResult> SetMainAsync(int imageId, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@ImageId", imageId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_PropertyImage_SetMain", p, commandType: CommandType.StoredProcedure);

        return new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
    }

    public async Task<ProcResult> ReorderAsync(int propertyId, IReadOnlyList<int> orderedImageIds, int actorAgentId)
    {
        using var db = _factory.Create();
        db.Open();
        using var tx = db.BeginTransaction();

        try
        {
            for (int i = 0; i < orderedImageIds.Count; i++)
            {
                var p = new DynamicParameters();
                p.Add("@ImageId", orderedImageIds[i]);
                p.Add("@PropertyId", propertyId);
                p.Add("@SortOrder", i);
                p.Add("@ActorAgentId", actorAgentId);
                p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

                await db.ExecuteAsync("dbo.usp_PropertyImage_SetOrder", p, transaction: tx, commandType: CommandType.StoredProcedure);

                var code = p.Get<int>("@ResponseCode");
                if (code != 0)
                {
                    tx.Rollback();
                    return new ProcResult(code, p.Get<string>("@ResponseMessage") ?? "Дараалал өөрчлөхөд алдаа гарлаа.");
                }
            }

            tx.Commit();
            return new ProcResult(0, "Дараалал шинэчлэгдлээ.");
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}