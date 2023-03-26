using Microsoft.EntityFrameworkCore;
using Toolbox.Api.Entities;

namespace Toolbox.Api.DbContext;
public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<AppInfo> AppInfo { get; set; } = null!;
    public DbSet<AppBase> AppBase { get; set; } = null!;
    public DbSet<AppRelease> AppRelease { get; set; } = null!;    
    public DbSet<Component> Component { get; set; } = null!;
    public DbSet<ConfigBase> ConfigBase { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}