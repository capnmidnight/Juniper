using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("roles")]
    public Task<IActionResult> GetRolesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from role in db.Roles
                 select new RoleModel(role)));

    [HttpGet("roles/users")]
    public Task<IActionResult> GetRoleUsersAsync() =>
        GetUserRolesAsync();

    [HttpGet($"roles/{{{nameof(roleId)}:int}}/users")]
    public Task<IActionResult> GetUsersInRoleAsync([FromRoute] int roleId) =>
        WithErrorHandlingAsync(() =>
            Json(from ur in db.UserRoles
                 where ur.RoleId == roleId
                 select new UserRoleModel(ur)));
}
