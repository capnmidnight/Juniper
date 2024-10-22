namespace Hjg.Pngcs.Chunks;

/// <summary>
/// general class for textual chunks
/// </summary>
public abstract class AbstractPngChunkTextVar : PngChunkMultiple
{
    protected internal string Key { get; set; } // key/val: only for tEXt. lazy computed

    protected internal string Val { get; set; }

    protected AbstractPngChunkTextVar(string id, ImageInfo info)
        : base(id, info)
    {
    }

    public const string KEY_Title = "Title"; // Short (one line) title or caption for image
    public const string KEY_Author = "Author"; // Name of image's creator
    public const string KEY_Description = "Description"; // Description of image (possibly long)
    public const string KEY_Copyright = "Copyright"; // Copyright notice
    public const string KEY_Creation_Time = "Creation Time"; // Time of original image creation
    public const string KEY_Software = "Software"; // Software used to create the image
    public const string KEY_Disclaimer = "Disclaimer"; // Legal disclaimer
    public const string KEY_Warning = "Warning"; // Warning of nature of content
    public const string KEY_Source = "Source"; // Device used to create the image
    public const string KEY_Comment = "Comment"; // Miscellaneous comment

    public override ChunkOrderingConstraint GetOrderingConstraint()
    {
        return ChunkOrderingConstraint.NONE;
    }

    public void SetKeyVal(string key, string val)
    {
        Key = key;
        Val = val;
    }
}