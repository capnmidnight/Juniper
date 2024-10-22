using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("users")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetUsersAsync()
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from user in db.Users.AsEnumerable()
                 select user.Memo(memo, () => new UserModel(user, memo))));
    }

    [HttpDelete($"users/{{{nameof(userId)}:int}}")]
    public Task<IActionResult> DeleteUserAsync([FromRoute] int userId) =>
        WithUserAsync(async adminUser =>
        {
            adminUser.AssertAdmin();
            var user = await db.GetUserAsync(userId);
            db.DeleteUser(user, adminUser);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpGet("users/roles")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetUserRolesAsync()
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from ur in db.UserRoles.AsEnumerable()
                 select ur.Memo(memo, () => new UserRoleModel(ur, memo))));
    }

    [HttpGet($"users/{{{nameof(userId)}:int}}/roles")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetRolesForUserAsync([FromRoute] int userId)
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from ur in db.UserRoles.AsEnumerable()
                 where ur.UserId == userId
                 select ur.Memo(memo, () => new UserRoleModel(ur, memo))));
    }

    [HttpPost($"users/{{{nameof(userId)}:int}}/roles")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> AddUserToRoleAsync([FromRoute] int userId, [FromBody] int roleId) =>
        WithUserAsync(async adminUser =>
        {
            adminUser.AssertAdmin();
            var user = await db.GetUserAsync(userId);
            var role = await db.GetRoleAsync(roleId);
            var userRole = db.AddUserToRole(user, role, adminUser);
            await db.SaveChangesAsync();
            return Json(new UserRoleModel(userRole));
        });

    [HttpDelete($"users/{{{nameof(userId)}:int}}/roles/{{{nameof(roleId)}:int}}")]
    public Task<IActionResult> RemoveRoleFromUserAsync([FromRoute] int userId, [FromRoute] int roleId) =>
        WithUserAsync(async adminUser =>
        {
            adminUser.AssertAdmin();
            var user = await db.GetUserAsync(userId);
            var role = await db.GetRoleAsync(roleId);
            await db.RemoveUserFromRoleAsync(user, role, adminUser);
            await db.SaveChangesAsync();
            return Ok();
        });

    public record SetUserInput(
        string UserName,
        string Email
    );
    [HttpPost($"users")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetUserAsync([FromBody] SetUserInput input) =>
        WithUserAsync(async adminUser =>
        {
            adminUser.AssertAdmin();
            var user = await db.SetUserAsync(input.UserName, input.Email);
            await db.SaveChangesAsync();
            return Json(new UserModel(user));
        });

    [HttpPost($"users/grant")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GrantUserAccessAsync([FromBody] int userId) =>
        WithUserAsync(async adminUser =>
        {
            var user = await db.GetUserAsync(userId);
            db.AddUserToRole(user, db.UserRole, adminUser);
            await db.SaveChangesAsync();
            return Ok();
        });
}
