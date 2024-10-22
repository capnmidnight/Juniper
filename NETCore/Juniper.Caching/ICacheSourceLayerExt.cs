using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Caching;


public static class ICacheSourceLayerExt
{
    public static Task<Stream?> OpenAsync(
        this ICacheSourceLayer layer,
        ContentReference fileRef)
    {
        if (layer is null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        return layer.GetStreamAsync(fileRef, null);
    }

    public static async Task<ResultT?> LoadAsync<ResultT, M>(
        this ICacheSourceLayer layer,
        IDeserializer<ResultT, M> deserializer,
        ContentReference fileRef,
        IProgress? prog = null)
        where M : MediaType
    {
        if (layer is null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        if (deserializer is null)
        {
            throw new ArgumentNullException(nameof(deserializer));
        }

        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        var progs = prog.Split("Read", "Decode");
        using var stream = await layer
            .GetStreamAsync(fileRef, progs[0])
            .ConfigureAwait(false)
            ?? throw new FileNotFoundException(fileRef.FileName);

        using var progStream = new ProgressStream(stream, stream.Length, progs[1], false);
        return deserializer.Deserialize(progStream);
    }

    public static bool TryLoad<ResultT, M>(
        this ICacheSourceLayer layer,
        IDeserializer<ResultT, M> deserializer,
        ContentReference fileRef,
        out ResultT? value,
        IProgress? prog = null)
        where M : MediaType
        where ResultT : class
    {
        value = default;

        var task = layer.LoadAsync(deserializer, fileRef, prog);
        Task.WhenAny(task).Wait();

        if (task.Status == TaskStatus.RanToCompletion)
        {
            value = task.Result;
        }

        return value != default;
    }

    public static bool TryLoadJson<ResultT>(
        this ICacheSourceLayer layer,
        string name,
        out ResultT? value,
        IProgress? prog = null)
        where ResultT : class
    {
        var deserializer = new JsonFactory<ResultT>();
        var fileRef = new ContentReference(name, MediaType.Application_Json);

        return layer.TryLoad(deserializer, fileRef, out value, prog);
    }


    /// <summary>
    /// Retrieve all the content references that can be deserialized by the
    /// given deserializer.
    /// </summary>
    /// <typeparam name="ResultT"></typeparam>
    /// <typeparam name="MediaTypeT"></typeparam>
    /// <param name="deserializer"></param>
    /// <returns></returns>
    public static IEnumerable<(ContentReference contentRef, ResultT result)> Get<ResultT, MediaTypeT>(
        this ICacheSourceLayer source,
        IFactory<ResultT, MediaTypeT> deserializer)
        where ResultT : class
        where MediaTypeT : MediaType
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (deserializer is null)
        {
            throw new ArgumentNullException(nameof(deserializer));
        }

        foreach (var contentRef in source.GetContentReferences(deserializer.OutputContentType))
        {
            if (source.TryLoad(deserializer, contentRef, out var result)
                && result is not null)
            {
                yield return (contentRef, result);
            }
        }
    }

    public static async Task<Dictionary<ContentReference, ResultT>> GetAsync<ResultT, MediaTypeT>(
        this ICacheSourceLayer source,
        IFactory<ResultT, MediaTypeT> deserializer,
        IProgress? prog = null)
        where ResultT : class
        where MediaTypeT : MediaType
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (deserializer is null)
        {
            throw new ArgumentNullException(nameof(deserializer));
        }

        var items = new Dictionary<ContentReference, ResultT>();
        var refs = source.GetContentReferences(deserializer.OutputContentType).ToArray();
        foreach ((var itemProg, var contentRef) in prog.Zip(refs))
        {
            var stream = await source
                .GetStreamAsync(contentRef, itemProg)
                .ConfigureAwait(false);

            if (stream is not null
                && deserializer.TryDeserialize(stream, out var value)
                && value is not null)
            {
                items.Add(contentRef, value);
            }
        }

        return items;
    }
}
