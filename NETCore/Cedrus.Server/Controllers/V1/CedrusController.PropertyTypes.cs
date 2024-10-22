using Juniper.Cedrus.Entities;
using Juniper.Units;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("properties/types")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetPropertyTypesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from v in db.FindPropertyTypes()
                 select new PropertyTypeModel(v)));



    public record SetPropertyTypeInput(
        string Name,
        DataType Type,
        StorageType Storage,
        UnitsCategory Category,
        string Description
    );
    [HttpPost("properties/types")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetPropertyTypeAsync([FromBody] SetPropertyTypeInput input) =>
        WithErrorHandlingAsync(async () =>
        {
            var p = await db.SetPropertyTypeAsync(input.Type, input.Storage, input.Name, input.Description, input.Category);
            await db.SaveChangesAsync();
            return Json(new PropertyTypeModel(p));
        });


    [HttpDelete($"properties/types/{{{nameof(propertyTypeId)}:int}}")]
    public Task<IActionResult> DeletePropertyTypeAsync([FromRoute] int propertyTypeId) =>
        WithErrorHandlingAsync(async () =>
        {
            var p = await db.GetPropertyTypeAsync(propertyTypeId);
            db.DeletePropertyType(p);
            await db.SaveChangesAsync();
            return Ok();
        });
}
