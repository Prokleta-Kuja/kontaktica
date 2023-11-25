using Hangfire;
using kontaktica.Models;
using Microsoft.AspNetCore.Mvc;

namespace kontaktica.Controllers;

[ApiController]
public class ContactController : ControllerBase
{
    readonly IBackgroundJobClient _jobClient;
    readonly ILogger<ContactController> _logger;
    public ContactController(ILogger<ContactController> logger, IBackgroundJobClient jobClient)
    {
        _jobClient = jobClient;
        _logger = logger;
    }

    [HttpGet(C.Routes.Root)]
    public IActionResult Root()
    {
        var allowed = HttpContext.DynDnsCheck();
        return allowed ? Redirect("/jobs") : NotFound();
    }
    [HttpGet(C.Routes.Healthcheck)]
    public IActionResult Healthcheck()
    {
        return Ok();
    }

    [HttpGet(C.Routes.IcaWeb)]
    public IActionResult IcaWeb(IcaWebRequest req)
    {
        _logger.LogInformation("ICA new contact request");
        _jobClient.Enqueue<Jobs.IcaWeb>(j => j.RunAsync(req, CancellationToken.None));
        return Accepted();
    }

    [HttpGet(C.Routes.ModWeb)]
    public IActionResult ModWeb(ModWebRequest req)
    {
        _logger.LogInformation("MetabuchMod new contact request");
        _jobClient.Enqueue<Jobs.ModWeb>(j => j.RunAsync(req, CancellationToken.None));
        return Accepted();
    }

    [HttpGet(C.Routes.CikloWeb)]
    public IActionResult CikloWeb(CikloWebRequest req)
    {
        _logger.LogInformation("Ciklo-Sport new buy request");
        _jobClient.Enqueue<Jobs.CikloWeb>(j => j.RunAsync(req, CancellationToken.None));
        return Accepted();
    }

    [HttpGet(C.Routes.CikloWebService)]
    public IActionResult CikloWebService(CikloWebServiceRequest req)
    {
        _logger.LogInformation("Ciklo-Sport new bike service request");
        _jobClient.Enqueue<Jobs.CikloWebService>(j => j.RunAsync(req, CancellationToken.None));
        return Accepted();
    }
}
