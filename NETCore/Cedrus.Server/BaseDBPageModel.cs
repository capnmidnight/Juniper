using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Juniper.Cedrus;

public abstract class BaseDBPageModel : PageModel
{
    protected CedrusContextSecure Database { get; }
    protected UserManager<CedrusUser> UserManager { get; }
    protected ILogger Logger { get; }
    public CedrusUser? IdentityUser { get; private set; }

    protected BaseDBPageModel(IServiceProvider services)
    {
        Database = services.GetRequiredService<CedrusContextSecure>();
        UserManager = services.GetRequiredService<UserManager<CedrusUser>>();
        Logger = services.GetRequiredService<ILogger<BaseDBPageModel>>();
    }

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        IdentityUser = await UserManager.GetTypedUserAsync(User, async (userName, email, primarySID) =>
        {
            var user = await Database.SetUserAsync(userName, email, primarySID);
            await Database.SaveChangesAsync();
            return user;
        });

        ViewData["IdentityUser"] = IdentityUser;

        await base.OnPageHandlerExecutionAsync(context, next);
    }

    protected async Task<IActionResult> WithErrorHandlingAsync(Func<Task<IActionResult>> act)
    {
        try
        {
            return await act();
        }
        catch (UnauthorizedAccessException exp)
        {
            Logger.LogWarning(exp, "Unauthorized: {message}", exp.Message);
            return Unauthorized();
        }
        catch (FileNotFoundException exp)
        {
            Logger.LogWarning(exp, "File not found: {message}", exp.Message);
            return NotFound(exp.FileName ?? "Unknown file");
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
            return Unauthorized();
        }
        catch (FileNotFoundException exp)
        {
            Logger.LogWarning(exp, "File not found: {message}", exp.Message);
            return NotFound(exp.FileName ?? "Unknown file");
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
        WithErrorHandlingAsync(() =>
            act(IdentityUser ?? throw new UnauthorizedAccessException()));

    protected IActionResult WithUser(Func<CedrusUser, IActionResult> act) =>
        WithErrorHandling(() =>
            act(IdentityUser ?? throw new UnauthorizedAccessException()));
}
