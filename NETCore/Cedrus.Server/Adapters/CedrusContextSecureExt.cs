using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;

using Microsoft.AspNetCore.Http;

namespace Juniper.Cedrus.Adapters;

public static class CedrusContextSecureExt
{

    public static async Task<FileAsset> UpsertFileAsync(this CedrusContextSecure context, IFormFile f, CedrusUser user)
    {
        using var mem = new MemoryStream((int)f.Length);
        await f.CopyToAsync(mem);
        await mem.FlushAsync();
        var bytes = mem.ToArray();
        var mediaType = MediaType.Parse(f.ContentType);
        return await context.UpsertFileAsync(f.FileName, bytes, mediaType, user);
    }
}
