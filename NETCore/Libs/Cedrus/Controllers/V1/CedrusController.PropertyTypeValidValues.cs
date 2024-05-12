using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("properties/types/values")]
    public Task<IActionResult> GetPropertyTypeValidValuesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from v in db.GetPropertyTypeValidValues()
                 select new PropertyTypeValidValueModel(v)));

    [HttpGet($"properties/types/{{{nameof(propertyTypeId)}:int}}/values")]
    public Task<IActionResult> GetPropertyTypeValidValuesAsync([FromRoute] int propertyTypeId) =>
        WithErrorHandlingAsync(() =>
            Json(from v in db.GetPropertyTypeValidValues(propertyTypeId)
                 select new PropertyTypeValidValueModel(v)));

    [HttpGet($"properties/types/{{{nameof(propertyTypeId)}:int}}/values")]
    public Task<IActionResult> SetPropertyTypeValidValuesAsync([FromRoute] int propertyTypeId, [FromBody]string value) =>
        WithErrorHandlingAsync(async () =>
        {
            var propertyType = await db.GetPropertyTypeAsync(propertyTypeId);
            return Json(new PropertyTypeValidValueModel(await db.SetPropertyTypeValidValueAsync(propertyType, value)));
        });
}
