using Toolbox.Api.Entities;

namespace Toolbox.Api.Interface.Repositories;
public interface IComponentRepository
{
    public Task<List<Component>> GetAllAsync();
}