using Toolbox.Api.Entities;

namespace Toolbox.Api.Interface;
public interface IAppBaseRepository
{
    public Task<List<AppBase>> GetAllAsync();

    public Task<AppBase?> GetOneAsync(string appKey);

    public Task<int> AddAsync(AppBase appBase);

    public Task<int> DeleteAsync(int id);

    public Task DownloadOnceAsync(string appKey);
}