using Microsoft.EntityFrameworkCore;
using Toolbox.Api.DbContext;
using Toolbox.Api.Entities;
using Toolbox.Api.Interface;

namespace Toolbox.Api.Repositories;
public class ComponentRepository : IComponentRepository
{
    private readonly AppDbContext _dbContext;
    public ComponentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Component>> GetAllAsync()
    {
        return await _dbContext.Component.OrderBy(x => x.Id).ToListAsync();
    }
}