
using MythApi.Common.Database.Models;

namespace MythApi.Mythologies.Interfaces;

public interface IMythologyRepository
{
    public Task<IList<Mythology>> GetAllMythologiesAsync();
    public Task<Mythology?> GetMythologyByIdAsync(int id);
}
