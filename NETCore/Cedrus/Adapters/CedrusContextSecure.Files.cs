using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

using System.Text;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public string GetHtml(FileAsset file, bool embed = true)
    {
        if (embed && file.MediaType.Matches(MediaType.Image.AnyImage))
        {
            return $@"<img title=""{file.Name}"" src=""{file.LinkPath}"">";
        }

        if (embed && file.MediaType.Matches(MediaType.Audio.AnyAudio))
        {
            return $@"<audio src=""{file.LinkPath}""></audio>";
        }

        if (embed && file.MediaType.Matches(MediaType.Video.AnyVideo))
        {
            return $@"<video src=""{file.LinkPath}""></video>";
        }

        if (!embed || !file.MediaType.Matches(MediaType.Text.AnyText))
        {
            return $@"<a href=""{file.LinkPath}"" download=""{file.MediaType.AddExtension(file.Name)}"">{file.Name}</a>";
        }

        file.Datum ??= insecure.FilesData.SingleOrDefault(d => d.FileAssetId == file.Id);
        if (file.Datum is null)
        {
            return "FILE CONTENT NOT FOUND";
        }

        var text = Encoding.UTF8.GetString(file.Datum.Blob);
        if (!file.MediaType.Matches(MediaType.Text_Html))
        {
            text = $"<pre>{text}</pre>";
        }

        return text;
    }

    public string GetSearchHtml(FileAsset file, string searchTerm)
    {
        var html = GetHtml(file);
        if (searchTerm is not null)
        {
            var parts = searchTerm
                .ToLowerInvariant()
                .Split(' ')
                .Select(v => v.Trim())
                .ToArray();
            foreach (var search in parts)
            {
                var text = html.ToLowerInvariant();
                var location = text.Length;

                // Iterate backwards over the string so we don't have to adjust insertion
                // indexes as we insert new text into the string.
                while (location > search.Length)
                {
                    location = text.LastIndexOf(search, location);
                    if (location > -1)
                    {
                        var end = location + search.Length;
                        html = html[..location] + "<em>" + html[location..end] + "</em>" + html[end..];
                    }
                }
            }
        }

        return html;
    }

    public async Task<FileAsset> UpsertFileAsync(string name, byte[] bytes, MediaType mediaType, CedrusUser user)
    {
        name = ValidateString(nameof(name), name);

        var file = insecure.Files.Local.SingleOrDefault(f => f.Name == name)
                ?? await insecure.Files
                .Include(f => f.Datum)
                .Include(f => f.SearchIndex)
                .SingleOrDefaultAsync(f => f.Name == name);

        if (file is null)
        {
            await insecure.Files.AddAsync(file = new()
            {
                MediaType = mediaType,
                Name = name,
                Length = bytes.LongLength,
                User = user
            });
        }
        else
        {
            file.MediaType = mediaType;
            file.Name = name;
            file.Length = bytes.LongLength;
        }

        if (file.Datum is null)
        {
            await insecure.FilesData.AddAsync(file.Datum = new()
            {
                FileAsset = file,
                Blob = bytes
            });
        }
        else
        {
            file.Datum.Blob = bytes;
        }

        if (mediaType.Matches(MediaType.Text.AnyText))
        {
            var text = Encoding.UTF8.GetString(bytes);
            if (file.SearchIndex is null)
            {
                await insecure.FileSearchIndex.AddAsync(file.SearchIndex = new()
                {
                    File = file,
                    SearchableText = text.ToLowerInvariant()
                });
            }
            else
            {
                file.SearchIndex.SearchableText = text.ToLowerInvariant();
            }
        }

        return file;
    }

    public IQueryable<FileAsset> GetFiles(CedrusUser user) =>
        insecure.Files;

    public async Task<FileAsset> GetFileByGuidAsync(Guid fileGuid, CedrusUser user) =>
        await GetFiles(user).SingleOrDefaultAsync(f => f.Guid == fileGuid)
        ?? throw new FileNotFoundException($"FileAsset:{fileGuid}");

    public void DeleteFile(FileAsset file)
    {
        // Cascade the File deletion to properties referring to the file by download path.
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast --- this works on Sqlite. Should also work on Postgres, but can't test right now.
        var props = from p in insecure.Properties
                    where p.Value == file.LinkPath
                    select p;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
        if (props.Any())
        {
            insecure.Properties.RemoveRange(props);
        }

        insecure.Files.Remove(file);
    }
}
