namespace MythApi.Gods.Models;

public class GodInput {
    public int? Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int MythologyId { get; set; }
}

