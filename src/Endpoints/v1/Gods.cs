using Microsoft.AspNetCore.Mvc;
using MythApi.Gods.Interfaces;
using MythApi.Common.Database.Models;
using MythApi.Gods.Models;

namespace MythApi.Endpoints.v1;
public static class Gods {
    // Validation constants
    private const int MAX_NAME_LENGTH = 100;
    private const int MAX_DESCRIPTION_LENGTH = 1000;
    private const int MAX_BATCH_SIZE = 100;

    public static void RegisterGodEndpoints(this IEndpointRouteBuilder endpoints) {
        
        var gods = endpoints.MapGroup("/api/v1/gods");


        gods.MapGet("", GetAlllGods);
        gods.MapGet("{id}", (int id, IGodRepository repository) => repository.GetGodAsync(new GodParameter(id)));
        gods.MapGet("search/{name}", (string name, IGodRepository repository, [FromQuery] bool includeAliases = false) => repository.GetGodByNameAsync(new GodByNameParameter(name, includeAliases)));
        gods.MapPost("", AddOrUpdateGods);
    }

    /// <summary>
    /// Adds or updates gods in the database with comprehensive input validation.
    /// </summary>
    /// <remarks>
    /// Security Note: This API validates input format and length but does not sanitize HTML/script content.
    /// API consumers are responsible for properly encoding/sanitizing data when rendering it in HTML contexts
    /// to prevent XSS attacks. This follows the principle that APIs should store data as-is and let clients
    /// handle presentation-layer security concerns.
    /// </remarks>
    public static async Task<IResult> AddOrUpdateGods(List<GodInput> gods, IGodRepository repository)
    {
        // Validate input list
        if (gods == null || gods.Count == 0)
        {
            return Results.BadRequest(new { error = "Input list cannot be null or empty." });
        }

        // Validate batch size
        if (gods.Count > MAX_BATCH_SIZE)
        {
            return Results.BadRequest(new { error = $"Batch size cannot exceed {MAX_BATCH_SIZE}." });
        }

        // Validate each god
        for (int i = 0; i < gods.Count; i++)
        {
            var god = gods[i];

            // Validate Name is not empty
            if (string.IsNullOrWhiteSpace(god.Name))
            {
                return Results.BadRequest(new { error = $"God at index {i}: Name cannot be empty." });
            }

            // Validate Name length
            if (god.Name.Length > MAX_NAME_LENGTH)
            {
                return Results.BadRequest(new { error = $"God at index {i}: Name must not exceed {MAX_NAME_LENGTH} characters." });
            }

            // Validate Description is not null
            if (god.Description == null)
            {
                return Results.BadRequest(new { error = $"God at index {i}: Description cannot be null." });
            }

            // Validate Description length
            if (god.Description.Length > MAX_DESCRIPTION_LENGTH)
            {
                return Results.BadRequest(new { error = $"God at index {i}: Description must not exceed {MAX_DESCRIPTION_LENGTH} characters." });
            }

            // Validate MythologyId is positive
            if (god.MythologyId <= 0)
            {
                return Results.BadRequest(new { error = $"God at index {i}: MythologyId must be a positive integer." });
            }

            // Validate Id is positive when provided for updates
            if (god.Id.HasValue && god.Id.Value <= 0)
            {
                return Results.BadRequest(new { error = $"God at index {i}: Id must be a positive integer when provided." });
            }
        }

        var result = await repository.AddOrUpdateGods(gods);
        return Results.Ok(result);
    }

    public static Task<IList<God>> GetAlllGods(IGodRepository repository) => repository.GetAllGodsAsync();
}