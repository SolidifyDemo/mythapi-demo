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

    /// <summary>
    /// Searches for gods by name using a case-insensitive LIKE pattern match.
    /// Optionally includes searching through god aliases.
    /// </summary>
    /// <param name="parameter">Search parameter containing the name to search for and whether to include aliases.</param>
    /// <returns>A list of gods matching the search criteria. Returns an empty list if no matches are found.</returns>
    /// <remarks>
    /// This method uses Entity Framework's parameterized queries (EF.Functions.Like) to prevent SQL injection attacks.
    /// User input is automatically sanitized through query parameterization.
    /// </remarks>
    public async Task<List<God>> GetGodByNameAsync(GodByNameParameter parameter)
    {
        var normalizedName = $"%{parameter.Name}%";
        
        if (parameter.IncludeAliases)
        {
            var gods = await _context.Gods
                .Include(g => g.Aliases)
                .Where(g => EF.Functions.Like(g.Name, normalizedName) 
                         || g.Aliases.Any(a => EF.Functions.Like(a.Name, normalizedName)))
                .ToListAsync();
            return gods;
        }
        else
        {
            return await _context.Gods
                .Where(g => EF.Functions.Like(g.Name, normalizedName))
                .ToListAsync();
        }
    }
}