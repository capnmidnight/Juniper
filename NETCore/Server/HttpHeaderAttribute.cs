using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Juniper;

public class HttpHeaderAttribute : Attribute, IActionConstraint
{
    public string Header { get; set; }
    public string Value { get; set; }
    public int Order { get; set; }

    public HttpHeaderAttribute(string header, string value)
    {
        Header = header;
        Value = value;
    }

    public bool Accept(ActionConstraintContext context)
    {
        if (context.RouteContext.HttpContext.Request.Headers.TryGetValue(Header, out var value))
        {
            return value[0] == Value;
        }

        return false;
    }
}
