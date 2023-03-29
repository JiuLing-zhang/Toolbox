using Microsoft.EntityFrameworkCore;
using Toolbox.Api.DbContext;
using Toolbox.Api.Entities;
using Toolbox.Api.Enums;
using Toolbox.Api.Interface.Repositories;

namespace Toolbox.Api.Repositories;
public class AppReleaseRepository : IAppReleaseRepository
{
    private readonly AppDbContext _dbContext;
    public AppReleaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppRelease?> GetLastVersionAsync(string appKey, PlatformEnum platform)
    {
        return await _dbContext.AppRelease.OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync(x => x.AppKey == appKey && x.Platform == platform.ToString());
    }

    public async Task<int> AddAsync(AppRelease appInfo)
    {
        _dbContext.AppRelease.Add(appInfo);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AppRelease>> GetAllAsync()
    {
        return await _dbContext.AppRelease.Where(x => x.IsEnabled).ToListAsync();
    }
}