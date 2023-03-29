using Toolbox.Api.Entities;
using Toolbox.Api.Enums;

namespace Toolbox.Api.Interface;
public interface IAppInfoRepository
{
    public Task<List<AppInfo>> GetAllAsync();

    public Task<AppInfo?> GetOneAsync(string id);

    public Task<int> AddAsync(AppInfo appInfo);

    public Task<int> UpdateAsync(AppInfo appInfo);

    public Task<int> DeleteAsync(int id);

    public Task<AppInfo?> QueryOneApp(string appKey, PlatformEnum platform);

    public Task<bool> Exist(string id);
}