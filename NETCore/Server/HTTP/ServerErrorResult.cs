using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace Juniper.HTTP;

public class ServerErrorResult : StatusCodeResult
{
    public ServerErrorResult() : base((int)HttpStatusCode.InternalServerError)
    {

    }
}
