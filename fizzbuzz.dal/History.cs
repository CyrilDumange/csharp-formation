using System.Data;
using System.Text.Json;
using Dapper;
using fizzbuzz.models;

namespace fizzbuzz.dal;

public class History(IDbConnection _connection) : IHistory
{
    public async Task Count(FizzBuzzInput input)
    {
        var j = JsonSerializer.Serialize(input);
        await _connection.ExecuteAsync(@"INSERT INTO fizz_history(input, count) VALUES(@input, 1)
            ON CONFLICT(input) DO
            UPDATE SET count = excluded.count + 1", new { input = j });
    }

    public Task<int> GetCount(FizzBuzzInput input)
    {
        var j = JsonSerializer.Serialize(input);
        return _connection.QuerySingleAsync<int>("SELECT count FROM fizz_history WHERE input = @input", new { input = j });
    }
}
