using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("units")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetUnitsData() =>
        WithErrorHandling(() =>
            Json(Units.Converter.UnitsByCategory));

    [HttpGet("units/abbreviations")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetUnitAbbreviations() =>
        WithErrorHandling(() =>
            Json(Units.Converter.Abbreviations));

    [HttpPost($"units/convert/{{{nameof(from)}}}/{{{nameof(to)}}}")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult ConvertUnits([FromRoute] Units.UnitOfMeasure from, [FromRoute] Units.UnitOfMeasure to, [FromBody] float value) =>
        WithErrorHandling(() =>
            Json(Units.Converter.Convert(value, from, to)));
}
