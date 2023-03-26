using Toolbox.Api.Entities;
using Toolbox.Api.Enums;

namespace Toolbox.Api.Interface.Repositories;
public interface IAppReleaseRepository
{
    public Task<AppRelease?> GetLastVersionAsync(string appKey, PlatformEnum platform);
    public Task<int> AddAsync(AppRelease appRelease);
}