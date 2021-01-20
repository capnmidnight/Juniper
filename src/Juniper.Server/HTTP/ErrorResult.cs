using System;
using System.Net;

namespace Juniper.HTTP
{
    public class ErrorResult : ResultWithMessage
    {
        public ErrorResult(Exception exp)
            : base(HttpStatusCode.InternalServerError, exp.Message, MediaType.Text.Plain)
        { }
    }
}
