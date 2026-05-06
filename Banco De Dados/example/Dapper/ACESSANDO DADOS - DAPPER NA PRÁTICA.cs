public async Task<IEnumerable<PostDTO>> GetPostsByTitleV2Async(string title,CancellationToken = default)
{
var sql = @"SELECT Id,
				Body,
				CreationDate
				FROM Posts
				WHERE Title = @title";

DynamicParameters param = new();

param.Add("@title", title, DbType.AnsiString, ParameterDirection.Input)

await using SqlConnection connection = new(_connectionString);

return await connection
.QueryAsync<PostDTO>(sql, param)
.WaitAsync(cancellationToken)
.ConfigureAwait(false);
}