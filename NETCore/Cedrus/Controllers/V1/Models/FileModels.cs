using Juniper.Units;

using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class FileModel : AbstractClassificationMarked
{
    public string TypeStamp => "file";
    public string? Name { get; }
    public Guid? Guid { get; }
    public string? Type { get; }
    public string? Size { get; }
    public string? Path { get; }
    public UserModel User { get; }
    public DateTime? CreatedOn { get; }

    public FileModel(FileAsset? file)
        : base(file)
    {
        Name = file?.Name;
        Guid = file?.Guid;
        Type = file?.MediaType?.ToString();
        Size = FileSize.Format(file?.Length);
        Path = file?.LinkPath;
        User = new UserModel(file?.User);
        CreatedOn = file?.CreatedOn;
    }
}