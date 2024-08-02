using Microsoft.AspNetCore.Mvc;
using WolverineHoP.VanillaApi.Services;

namespace WolverineHoP.VanillaApi.Controllers;

[ApiController]
public class SeedDataController : ControllerBase
{
    private readonly IDatabaseConfigurationService _databaseConfigurationService;

    public SeedDataController(IDatabaseConfigurationService databaseConfigurationService)
    {
        _databaseConfigurationService = databaseConfigurationService;
    }

    /// <summary>
    /// Create the database schema
    /// </summary>
    [HttpPost("api/setup-schema")]
    public async Task<ActionResult> SetupDatabaseSchema(CancellationToken token)
    {
        await _databaseConfigurationService.SeedData(token);
        return Ok();
    }
}