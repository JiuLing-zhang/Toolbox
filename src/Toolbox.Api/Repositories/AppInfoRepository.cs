using Microsoft.EntityFrameworkCore;
using Toolbox.Api.DbContext;
using Toolbox.Api.Entities;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface;

namespace Toolbox.Api.Repositories;
internal class AppInfoRepository : IAppInfoRepository
{
    private readonly AppDbContext _dbContext;
    public AppInfoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AppInfo>> GetAllAsync()
    {
        return await _dbContext.AppInfo.Where(x => x.IsEnabled).ToListAsync();
    }

    public async Task<AppInfo?> GetOneAsync(string id)
    {
        return await _dbContext.AppInfo.FindAsync(id);
    }

    public async Task<int> AddAsync(AppInfo appInfo)
    {
        _dbContext.AppInfo.Add(appInfo);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(AppInfo appInfo)
    {
        _dbContext.AppInfo.Update(appInfo);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        var appInfo = await _dbContext.AppInfo.FindAsync(id);
        if (appInfo == null)
        {
            return 0;
        }
        _dbContext.AppInfo.Remove(appInfo);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<AppInfo?> QueryOneApp(string appKey, PlatformEnum platform)
    {
        return await _dbContext.AppInfo.Where(x => x.AppKey == appKey.ToLower() && x.Platform == platform.ToString() && x.IsEnabled).OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync();
    }

    public async Task<bool> Exist(string id)
    {
        var appInfo = await GetOneAsync(id);
        return appInfo != null;
    }
}