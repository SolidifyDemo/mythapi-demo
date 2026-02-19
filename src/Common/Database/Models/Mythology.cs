namespace MythApi.Common.Database.Models;
public class Mythology {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<God> Gods { get; set; } = [];
}
