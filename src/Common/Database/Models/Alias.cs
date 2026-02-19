namespace MythApi.Common.Database.Models;

public class Alias {
    public int Id { get; set; }
    public int GodId { get; set; }
    public string Name { get; set; } = null!;
}