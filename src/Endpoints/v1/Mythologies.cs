using MythApi.Common.Database.Models;
using MythApi.Mythologies.Interfaces;

public static class Mythologies
{
    public static void RegisterMythologiesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var mythologies = endpoints.MapGroup("/api/v1/mythologies");

        mythologies.MapGet("", GetAllMythologies);
    }

    public static Task<IList<Mythology>> GetAllMythologies(IMythologyRepository repository) => repository.GetAllMythologiesAsync();
}
