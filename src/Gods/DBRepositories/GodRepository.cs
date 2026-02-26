using Microsoft.EntityFrameworkCore;
using MythApi.Gods.Interfaces;
using MythApi.Common.Database.Models;
using MythApi.Common.Database;
using MythApi.Gods.Models;

namespace MythApi.Gods.DBRepositories;

public class GodRepository : IGodRepository
{
    private readonly AppDbContext _context;

    public GodRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<God>> AddOrUpdateGods(List<GodInput> gods)
    {
        foreach(var god in gods) {
            if (god.Id.HasValue && _context.Gods.Any(x => x.Id == god.Id))
            {
                _context.Gods.Where(x => x.Id == god.Id)
                    .ExecuteUpdate(setter => 
                        setter.SetProperty(x => x.Name, god.Name)
                            .SetProperty(x => x.Description, god.Description)
                        );
            }
            else
            {
                var newGod = new God
                {
                    Name = god.Name,
                    MythologyId = god.MythologyId,
                    Description = god.Description
                };
                _context.Gods.Add(newGod);
            }
        }

        await _context.SaveChangesAsync();
        return await _context.Gods.ToListAsync();
    }

    public async Task<IList<God>> GetAllGodsAsync()
    {
        var gods = await _context.Gods.ToListAsync();
        foreach (var god in gods)
        {
            _context.Entry(god).Collection(x => x.Aliases).Load();
        }
        return gods;
    }

    public async Task<God> GetGodAsync(GodParameter parameter)
    {
        return await _context.Gods.FirstAsync(x => x.Id == parameter.Id);
    }

    public Task<List<God>> GetGodByNameAsync(GodByNameParameter parameter)
    {
        var query = parameter.IncludeAliases ? $"SELECT * FROM God WHERE Name LIKE '%{parameter.Name}%' or Id in (SELECT GodId FROM Alias WHERE Name LIKE '%{parameter.Name}%')" : $"SELECT * FROM God WHERE Name LIKE '%{parameter.Name}%'";
        var result = _context.Gods.FromSqlRaw(query).ToList();

        return Task.FromResult(result);
    }

    /// <summary>
    /// Deletes all gods from the database.
    /// </summary>
    /// <returns>The number of gods deleted.</returns>
    /// <remarks>
    /// This is a destructive operation that permanently removes all god records.
    /// This operation should be restricted to authorized administrators only.
    /// </remarks>
    public async Task<int> DeleteAllGodsAsync()
    {
        var count = await _context.Gods.ExecuteDeleteAsync();
        
        if (count > 0)
        {
            Serilog.Log.Warning("Deleted all {Count} gods from the database", count);
        }
        
        return count;
    }
}