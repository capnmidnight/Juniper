using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Juniper.Cedrus.Controllers.V1;

[ApiController]
[Route("api/[controller]/v1")]
public partial class CedrusController : AbstractIdentityController
{
    private readonly CedrusContextSecure db;
    private readonly IEnumerable<EndpointDataSource> endpointSources;
    private readonly RoleManager<CedrusRole> roleMgr;

    public CedrusController(
        CedrusContextSecure db, 
        UserManager<CedrusUser> userMgr, 
        RoleManager<CedrusRole> roleMgr, 
        ILogger<CedrusController> logger,
        IEnumerable<EndpointDataSource> endpointSources)
        : base(userMgr, logger)
    {
        this.db = db;
        this.roleMgr = roleMgr;
        this.endpointSources = endpointSources;
    }


    [HttpGet("endpoints")]
    [HttpHeader("Accept", "application/json")]
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
}
