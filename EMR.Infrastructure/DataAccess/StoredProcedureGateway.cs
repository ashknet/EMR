using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace EMR.Infrastructure.DataAccess;

public interface IStoredProcedureGateway
{
    Task<DataTable> ExecuteToDataTableAsync(string storedProcedureName, Action<SqlParameterCollection>? buildParams, CancellationToken cancellationToken);
}

public class StoredProcedureGateway : IStoredProcedureGateway
{
    private readonly string _connectionString;

    public StoredProcedureGateway(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DataTable> ExecuteToDataTableAsync(string storedProcedureName, Action<SqlParameterCollection>? buildParams, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = new SqlCommand(storedProcedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        buildParams?.Invoke(cmd.Parameters);

        var table = new DataTable();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        table.Load(reader);
        return table;
    }
}

