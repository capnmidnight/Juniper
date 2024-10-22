using Juniper.Cedrus.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Juniper.Cedrus.Controllers.V1;

public abstract class AbstractIdentityController : Controller
{
    protected readonly ILogger Logger;
    private readonly UserManager<CedrusUser> UserManager;

    protected AbstractIdentityController(UserManager<CedrusUser> userMgr, ILogger logger)
    {
        UserManager = userMgr;
        Logger = logger;
    }

    protected async Task<IActionResult> Json<T>(IQueryable<T> query) =>
        base.Json(await query.ToArrayAsync());

    protected async Task<IActionResult> Json<T>(Task<IEnumerable<T>> collectionTask) =>
        Json(await collectionTask);

    protected IActionResult Json<T>(IEnumerable<T> query) =>
        Json(query.ToArray());

    protected IActionResult Json<T>(T[] query) =>
        base.Json(query);

    protected IActionResult Json(string value) =>
        base.Json(value);

    protected async Task<IActionResult> WithErrorHandlingAsync(Func<Task<IActionResult>> act)
    {
        try
        {
            return await act();
        }
        catch (UnauthorizedAccessException exp)
        {
            Logger.LogWarning(exp, "Unauthorized: {message}", exp.Message);
            return Unauthorized(exp.Message);
        }
        catch (FileNotFoundException exp)
        {
            Logger.LogWarning(exp, "File not found: {message}", exp.Message);
            return NotFound(exp.FileName);
        }
        catch (ArgumentException exp)
        {
            Logger.LogWarning(exp, "Bad request: {message}", exp.Message);
            return BadRequest(exp.Message);
        }
        catch (Exception exp)
        {
            Logger.LogError(exp, "Server Error: {message}", exp.Message);
            return StatusCode(500, exp.Message);
        }
    }

    protected IActionResult WithErrorHandling(Func<IActionResult> act)
    {
        try
        {
            return act();
        }
        catch (UnauthorizedAccessException exp)
        {
            Logger.LogWarning(exp, "Unauthorized: {message}", exp.Message);
            return Unauthorized(exp.Message);
        }
        catch (FileNotFoundException exp)
        {
            Logger.LogWarning(exp, "File not found: {message}", exp.Message);
            return NotFound(exp.FileName);
        }
        catch (ArgumentException exp)
        {
            Logger.LogWarning(exp, "Bad request: {message}", exp.Message);
            return BadRequest(exp.Message);
        }
        catch (Exception exp)
        {
            Logger.LogError(exp, "Server Error: {message}", exp.Message);
            return StatusCode(500, exp.Message);
        }
    }

    protected Task<IActionResult> WithUserAsync(Func<CedrusUser, Task<IActionResult>> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await UserManager.GetTypedUserAsync(User)
                ?? throw new UnauthorizedAccessException();
            return await act(user);
        });

    protected Task<IActionResult> WithUserAsync(Func<CedrusUser, IActionResult> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await UserManager.GetTypedUserAsync(User)
                ?? throw new UnauthorizedAccessException();
            return act(user);
        });

    protected Task<IActionResult> WithReturnUrlAsync(string? returnUrl, Func<Task> task) =>
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
