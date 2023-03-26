using Toolbox.Api.DbContext;
using Toolbox.Api.Interface.Repositories;

namespace Toolbox.Api.Repositories;
public class ConfigBaseRepository : IConfigBaseRepository
{
    private readonly AppDbContext _dbContext;
    public ConfigBaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetOneAsync<T>(string key)
    {
        var config = await _dbContext.ConfigBase.FindAsync(key);
        if (config == null)
        {
            return default;
        }
        return System.Text.Json.JsonSerializer.Deserialize<T>(config.ConfigDetail);
    }

    public async Task<string> GetOneAsync(string key)
    {
        var config = await _dbContext.ConfigBase.FindAsync(key);
        if (config == null)
        {
            return "";
        }
        return config.ConfigDetail;
    }
}
