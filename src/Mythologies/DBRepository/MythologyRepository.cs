
using Microsoft.EntityFrameworkCore;
using MythApi.Common.Database;
using MythApi.Common.Database.Models;
using MythApi.Mythologies.Interfaces;

namespace MythApi.Mythologies.DBRepositories;

public class MythologyRepository : IMythologyRepository
{
    private readonly AppDbContext _context;

    public MythologyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Mythology>> GetAllMythologiesAsync()
    {
        return await _context.Mythologies.ToListAsync();
    }
}
