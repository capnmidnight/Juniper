using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("roles")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetRolesAsync()
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from role in db.Roles.AsEnumerable()
                 select role.Memo(memo, () => new RoleModel(role))));
    }

    [HttpPost("roles")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> CreateRoleAsync([FromBody] string roleName) =>
        WithUserAsync(async adminUser =>
        {
            var role = await db.CreateRoleAsync(roleName, adminUser);
            await db.SaveChangesAsync();
            return Json(new RoleModel(role));
        });


    [HttpGet("roles/users")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetRoleUsersAsync() =>
        GetUserRolesAsync();

    [HttpGet($"roles/{{{nameof(roleId)}:int}}/users")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetUsersInRoleAsync([FromRoute] int roleId)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(() =>
            Json(from ur in db.UserRoles
                 where ur.RoleId == roleId
                 select ur.Memo(memo, () => new UserRoleModel(ur, memo))));
    }
}
