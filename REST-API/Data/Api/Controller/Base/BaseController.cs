using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace REST_API.Data.Api.Controller.Base;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
[Produces("application/json")]
public class BaseController<TContext, TController> : ControllerBase
{
    protected TContext Context { get; }
    protected ILogger<TController> Logger { get; set; }

    public BaseController(TContext context, ILogger<TController> logger)
    {
        Context = context;
        Logger = logger;
    }
}