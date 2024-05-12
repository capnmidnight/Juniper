using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("properties/types")]
    public Task<IActionResult> GetPropertyTypesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from v in db.GetPropertyTypes()
                 select new PropertyTypeModel(v)));


    [HttpPost("properties/types")]
    public Task<IActionResult> SetPropertyTypeAsync([FromBody] SetPropertyTypeInput input) =>
        WithErrorHandlingAsync(async () =>
        {
            var p = db.SetPropertyType(input.DataType, input.Name, input.Description, input.Category);
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
