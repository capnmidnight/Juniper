using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;
using Juniper.HTTP;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.Cedrus;

public abstract class BaseDBPageModel : PageModel
{
    protected CedrusContextSecure Database { get; private set; }
    protected UserManager<CedrusUser> UserManager { get; private set; }

    protected BaseDBPageModel(IServiceProvider services)
    {
        Database = services.GetRequiredService<CedrusContextSecure>();
        UserManager = services.GetRequiredService<UserManager<CedrusUser>>();
    }

    protected async Task<IActionResult> WithErrorHandlingAsync(Func<Task<IActionResult>> act)
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

    protected IActionResult WithErrorHandling(Func<IActionResult> act)
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

    protected Task<IActionResult> WithUserAsync(Func<CedrusUser, Task<IActionResult>> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await UserManager.GetTypedUser(User);
            if (user is null)
            {
                return Unauthorized();
            }
            return await act(user);
        });

    protected Task<IActionResult> WithUserAsync(Func<CedrusUser, IActionResult> act) =>
        WithErrorHandlingAsync(async () =>
        {
            var user = await UserManager.GetTypedUser(User);
            if (user is null)
            {
                return Unauthorized();
            }
            return act(user);
        });
}
