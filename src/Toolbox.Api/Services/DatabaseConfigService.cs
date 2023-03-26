using Toolbox.Api.Interface.Repositories;
using Toolbox.Api.Interface.Services;

namespace Toolbox.Api.Services;
public class DatabaseConfigService : IDatabaseConfigService
{
    private readonly IConfigBaseRepository _configBaseRepository;
    public DatabaseConfigService(IConfigBaseRepository configBaseRepository)
    {
        _configBaseRepository = configBaseRepository;
    }
    public async Task<T?> GetOneAsync<T>(string key)
    {
        return await _configBaseRepository.GetOneAsync<T>(key);
    }

    public async Task<string> GetOneAsync(string key)
    {
        return await _configBaseRepository.GetOneAsync(key);
    }
}