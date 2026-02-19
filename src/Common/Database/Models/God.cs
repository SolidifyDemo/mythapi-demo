
namespace MythApi.Common.Database.Models;

public class God
{

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int MythologyId { get; set; }
    public List<Alias> Aliases { get; set; } = [];
}
