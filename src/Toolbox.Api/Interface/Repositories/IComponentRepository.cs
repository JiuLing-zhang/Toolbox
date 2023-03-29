using Toolbox.Api.Entities;

namespace Toolbox.Api.Interface;
public interface IComponentRepository
{
    public Task<List<Component>> GetAllAsync();
}