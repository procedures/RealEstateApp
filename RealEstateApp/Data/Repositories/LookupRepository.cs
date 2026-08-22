using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface ILookupRepository
{
    Task<LookupData> GetAllAsync();
}

public class LookupRepository : ILookupRepository
{
    private readonly IDbConnectionFactory _factory;
    public LookupRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<LookupData> GetAllAsync()
    {
        using var db = _factory.Create();
        using var multi = await db.QueryMultipleAsync(
            "dbo.usp_Lookups_GetAll", commandType: CommandType.StoredProcedure);

        return new LookupData
        {
            Districts = (await multi.ReadAsync<District>()).ToList(),
            PropertyTypes = (await multi.ReadAsync<PropertyType>()).ToList(),
            TransactionTypes = (await multi.ReadAsync<TransactionType>()).ToList(),
            WindowDirections = (await multi.ReadAsync<WindowDirection>()).ToList(),
            ClosureReasons = (await multi.ReadAsync<ClosureReason>()).ToList(),
            Branches = (await multi.ReadAsync<Branch>()).ToList(),
        };
    }
}