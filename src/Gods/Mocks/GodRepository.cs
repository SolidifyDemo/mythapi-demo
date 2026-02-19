using MythApi.Gods.Interfaces;
using MythApi.Common.Database.Models;
using MythApi.Gods.Models;

namespace MythApi.Gods.Mocks;

public class GodRepository : IGodRepository
{
    private List<God> gods = new List<God>{
        new() {
            Name = "Zeus",
            Description = "Zeus is the sky and thunder god in ancient Greek religion, who rules as king of the gods of Mount Olympus.",
        },
        new() {
            Name = "Hades",
            Description = "Hades is the god of the dead and the king of the underworld, with which his name became synonymous.",
        },
        new() {
            Name = "Poseidon",
            Description = "Poseidon was god of the sea, earthquakes, storms, and horses and is considered one of the most bad-tempered, moody and greedy Olympian gods.",
        },
        new() {
            Name = "Athena",
            Description = "Athena is the goddess of handicrafts, useful arts, and battle strategy. She is the patron goddess of heroic endeavor.",
        },
        new() {
            Name = "Odin",
            Description = "Odin is a god in Norse mythology, who was associated with healing, death, knowledge, sorcery, poetry, battle and the runic alphabet.",
        }
    };

    public Task<List<God>> AddOrUpdateGods(List<GodInput> gods)
    {
        this.gods.AddRange(gods.Select(god => new God(){Name = god.Name, Description = god.Description}));
        return Task.FromResult(this.gods);
    }

    public Task<IList<God>> GetAllGodsAsync()
    {
        return Task.FromResult(gods as IList<God>);
    }

    public Task<God> GetGodAsync(GodParameter parameter)
    {
        return Task.FromResult(gods[parameter.Id]);
    }

    public Task<List<God>> GetGodByNameAsync(GodByNameParameter parameter)
    {
        return Task.FromResult(gods.Where(god => god.Name.Contains(parameter.Name)).ToList());
    }
}