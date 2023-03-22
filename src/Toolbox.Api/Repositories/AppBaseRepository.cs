using Microsoft.EntityFrameworkCore;
using Toolbox.Api.DbContext;
using Toolbox.Api.Entities;
using Toolbox.Api.Interface;

namespace Toolbox.Api.Repositories;
internal class AppBaseRepository : IAppBaseRepository
{
    private readonly AppDbContext _dbContext;
    public AppBaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AppBase>> GetAllAsync()
    {
        return await _dbContext.AppBase.OrderBy(x => x.Sort).ToListAsync();
    }

    public async Task<AppBase?> GetOneAsync(string appKey)
    {
        return await _dbContext.AppBase.FirstOrDefaultAsync(x => x.AppKey == appKey && x.IsShow);
    }

    public async Task<int> AddAsync(AppBase appBase)
    {
        _dbContext.AppBase.Add(appBase);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        var appBase = await _dbContext.AppBase.FindAsync(id);
        if (appBase == null)
        {
            return 0;
        }
        _dbContext.AppBase.Remove(appBase);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DownloadOnceAsync(string appKey)
    {
        var appBase = await _dbContext.AppBase.FirstOrDefaultAsync(x => x.AppKey == appKey);
        if (appBase == null)
        {
            return;
        }
        appBase.DownloadCount += 1;
        _dbContext.AppBase.Update(appBase);
        await _dbContext.SaveChangesAsync();
    }
}