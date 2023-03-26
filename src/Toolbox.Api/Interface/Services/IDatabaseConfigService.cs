namespace Toolbox.Api.Interface.Services;
public interface IDatabaseConfigService
{
    public Task<T?> GetOneAsync<T>(string key);
    public Task<string> GetOneAsync(string key);
}