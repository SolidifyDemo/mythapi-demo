using MythApi.Common.Database.Models;
using MythApi.Mythologies.Interfaces;

public static class Mythologies
{
    public static void RegisterMythologiesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var mythologies = endpoints.MapGroup("/api/v1/mythologies");

        mythologies.MapGet("", GetAllMythologies);
        mythologies.MapGet("{id}", GetMythologyById);
    }

    public static Task<IList<Mythology>> GetAllMythologies(IMythologyRepository repository) => repository.GetAllMythologiesAsync();

    public static async Task<IResult> GetMythologyById(int id, IMythologyRepository repository)
    {
        var mythology = await repository.GetMythologyByIdAsync(id);
        return mythology is null ? Results.NotFound() : Results.Ok(mythology);
    }
}
