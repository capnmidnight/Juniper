using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("users")]
    public Task<IActionResult> GetUsersAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from user in db.Users
                 select new UserModel(user)));


    [HttpDelete($"users/{{{nameof(userId)}:int}}")]
    public Task<IActionResult> DeleteUserAsync([FromRoute] int userId) =>
        WithUserAsync(async adminUser =>
        {
            var user = await db.GetUserAsync(userId);
            db.DeleteUser(user, adminUser);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpGet("users/roles")]
    public Task<IActionResult> GetUserRolesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from ur in db.UserRoles
                 select new UserRoleModel(ur)));

    [HttpGet($"users/{{{nameof(userId)}:int}}/roles")]
    public Task<IActionResult> GetRolesForUserAsync([FromRoute] int userId) =>
        WithErrorHandlingAsync(() =>
            Json(from ur in db.UserRoles
                 where ur.UserId == userId
                 select new UserRoleModel(ur)));

    [HttpGet($"users/{{{nameof(userId)}:int}}/roles/{{{nameof(roleId)}:int}}")]
    public Task<IActionResult> RemoveRoleFromUserAsync([FromRoute] int userId, [FromRoute] int roleId) =>
        WithUserAsync(async adminUser =>
        {
            var user = await db.GetUserAsync(userId);
            var role = await db.GetRoleAsync(roleId);
            await db.RemoveUserFromRoleAsync(user, role, adminUser);
            await db.SaveChangesAsync();
            return Ok();
        });
}
