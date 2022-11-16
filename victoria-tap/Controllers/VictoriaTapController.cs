using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Providers.Contracts;
using victoria_tap.Services.Contracts;

namespace victoria_tap.Controllers;

[ApiController]
[Route("[controller]")]
public class VictoriaTapController : ControllerBase
{
    private readonly ILogger<VictoriaTapController> _logger;
    private readonly IVictoriaTapService _victoriaTapService;
    private readonly IVictoriaValidatorHandler _victoriaValidatorHandler;

    public VictoriaTapController(
        ILogger<VictoriaTapController> logger, 
        IVictoriaTapService victoriaTapService,
        IVictoriaValidatorHandler victoriaValidationHandler)
    {
        _logger = logger;
        _victoriaTapService = victoriaTapService;
        _victoriaValidatorHandler = victoriaValidationHandler;
    }

    /// <summary>
    /// Creates a new dispenser
    /// </summary>
    /// <response code="200">Dispenser created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="500">Internal server error</response>
    /// <param name="request">Flow volume</param>
    /// <returns>CreateDispenserResponse</returns>
    [HttpPost("/dispenser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateDispenser([Required][FromBody] CreateDispenserRequest request)
    {
        // Potential improvement: implementation of Mediator to abstract through handlers depending on needs
        var validation = _victoriaValidatorHandler.Validate(request);

        if (validation != null)
        {
            return validation;
        }

        var response = _victoriaTapService.CreateDispenser(request.FlowVolume);
        return new CreatedAtRouteResult("/dispenser", response.Data);
    }

    /// <summary>
    /// Changes dispenser status
    /// </summary>
    /// <response code="200">Dispenser created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="409">Invalid status</response>
    /// <response code="500">Internal server error</response>
    /// <param name="request">Status, updated at and id</param>
    /// <returns>Bool</returns>
    [HttpPut("/dispenser/{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ChangeDispenserStatus([Required][FromBody] ChangeDispenserStatusApiRequest request, string id)
    {
        // Potential improvement: implementation of Mediator to abstract through handlers depending on needs
        var changeDispenserStatusRequest = new ChangeDispenserStatusRequest()
        {
            Status = request.Status,
            UpdatedAt = request.UpdatedAt,
            Id = id
        };

        var validation = _victoriaValidatorHandler.Validate(changeDispenserStatusRequest);

        if (validation != null)
        {
            return validation;
        }

        var status = changeDispenserStatusRequest.Status;
        var updatedAt = changeDispenserStatusRequest.UpdatedAt;

        var response = _victoriaTapService.ChangeDispenserStatus(status, updatedAt, id);
        return new AcceptedAtRouteResult($"/dispenser/{id}/status", response.Data);
    }

    /// <summary>
    /// Get dispenser spending info
    /// </summary>
    /// <response code="200">Dispenser created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">Not found</response>
    /// <response code="500">Internal server error</response>
    /// <param name="request">Dispenser id</param>
    /// <returns>DispenserSpendingInfoResponse</returns>
    [HttpGet("/dispenser/{id}/spending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetDispenserSpendingInfo(string id)
    {
        // Potential improvement: implementation of Mediator to abstract through handlers depending on needs
        var getDispenserSpendingInfoRequest = new GetDispenserSpendingInfoRequest() { Id = id };

        var validation = _victoriaValidatorHandler.Validate(getDispenserSpendingInfoRequest);

        if (validation != null)
        {
            return validation;
        }

        var response = _victoriaTapService.GetDispenserSpendingInfo(id);
        return new OkObjectResult(response.Data);
    }
}