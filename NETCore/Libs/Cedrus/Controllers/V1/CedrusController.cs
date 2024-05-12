using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;
using Juniper.HTTP;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Juniper.Cedrus.Controllers.V1;

[ApiController]
[Route("api/[controller]/v1")]
public partial class CedrusController : Controller
{
    private readonly CedrusContextSecure db;
    private readonly ILogger logger;
    private readonly IEnumerable<EndpointDataSource> endpointSources;
    private readonly UserManager<CedrusUser> userMgr;
    private readonly RoleManager<CedrusRole> roleMgr;

    public CedrusController(
        CedrusContextSecure db, 
        UserManager<CedrusUser> userMgr, 
        RoleManager<CedrusRole> roleMgr, 
        ILogger<CedrusController> logger,
        IEnumerable<EndpointDataSource> endpointSources)
    {
        this.db = db;
        this.userMgr = userMgr;
        this.roleMgr = roleMgr;
        this.logger = logger;
        this.endpointSources = endpointSources;
    }



    [HttpGet("endpoints")]
    public IActionResult ListAllEndpointsAsync()
    {
        var endpoints = from es in endpointSources
                        from ep in es.Endpoints
                        where ep is RouteEndpoint
                        select (RouteEndpoint)ep;

        var output = endpoints.Select(
            e =>
            {
                var controller = e.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();

                var methodInfo = e.Metadata
                    .OfType<HttpMethodMetadata>()
                    .FirstOrDefault();

                var methods = methodInfo
                    ?.HttpMethods
                    ?.ToArray()
                    ?.Join(", ");

                return new
                {
                    Methods = methods,
                    Route = $"/{e.RoutePattern.RawText?.TrimStart('/')}",
                    Action = e.DisplayName
                };
            }
        );

        return Json(output);
    }

    private async Task<IActionResult> Json<T>(IQueryable<T> query) =>
        base.Json(await query.ToArrayAsync());

    private async Task<IActionResult> Json<T>(Task<IEnumerable<T>> collectionTask) =>
        Json(await collectionTask);

    private IActionResult Json<T>(IEnumerable<T> query) =>
        Json(query.ToArray());

    private IActionResult Json<T>(T[] query) =>
        base.Json(query);

    private async Task<IActionResult> WithErrorHandlingAsync(Func<Task<IActionResult>> act)
    {
        try
        {
            return await act();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch
        {
            return new ServerErrorResult();
        }
    }

    private IActionResult WithErrorHandling(Func<IActionResult> act)
    {
        try
        {
            return act();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch
        {
            return new ServerErrorResult();
        }
    }

    private Task<IActionResult> WithUserAsync(Func<CedrusUser, Task<IActionResult>> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await userMgr.GetTypedUser(User);
            if (user is null)
            {
                return Unauthorized();
            }

            return await act(user);
        });

    private Task<IActionResult> WithUserAsync(Func<CedrusUser, IActionResult> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await userMgr.GetTypedUser(User);
            if (user is null)
            {
                return Unauthorized();
            }

            return act(user);
        });

    private Task<IActionResult> WithReturnUrlAsync(string? returnUrl, Func<Task> task) =>
        WithErrorHandlingAsync(async () =>
        {
            await task();
            if (returnUrl is null)
            {
                return Ok();
            }
            else
            {
                return Redirect(returnUrl);
            }
        });
}
