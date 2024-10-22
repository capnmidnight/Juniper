using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("properties/types/values")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetPropertyTypeValidValuesAsync()
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from v in db.GetPropertyTypeValidValues().AsEnumerable()
                 select v.Memo(memo, () => new PropertyTypeValidValueModel(v, memo))));
    }

    [HttpGet($"properties/types/{{{nameof(propertyTypeId)}:int}}/values")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetPropertyTypeValidValuesAsync([FromRoute] int propertyTypeId)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(async () => {
            var propertyType = await db.GetPropertyTypeAsync(propertyTypeId);
            return Json(from v in db.GetPropertyTypeValidValues(propertyType).AsEnumerable()
                        select v.Memo(memo, () => new PropertyTypeValidValueModel(v, memo)));
        });
    }

    [HttpGet($"properties/types/{{{nameof(propertyTypeId)}:int}}/values")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetPropertyTypeValidValuesAsync([FromRoute] int propertyTypeId, [FromBody] string[] values)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(async () =>
        {
            var propertyType = await db.GetPropertyTypeAsync(propertyTypeId);
            var validValues = (await db.SetPropertyTypeValidValueAsync(propertyType, values)).ToArray();
            await db.SaveChangesAsync();
            return Json(from v in validValues
                        select v.Memo(memo, () => new PropertyTypeValidValueModel(v, memo)));
        });
    }

    [HttpDelete($"properties/types/values/{{{nameof(validValueId)}:int}}")]
    public Task<IActionResult> DeletePropertyTypeValidValueAsync([FromRoute] int validValueId) =>
        WithErrorHandlingAsync(async () =>
        {
            var validValue = await db.GetPropertyTypeValidValueAsync(validValueId);
            db.DeletePropertyTypeValidValue(validValue);
            await db.SaveChangesAsync();
            return Ok();
        });
}
