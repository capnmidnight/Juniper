using Juniper.Units;

using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class FileModel : AbstractSequencedModel
{
    public string TypeStamp => "file";
    public string Name { get; }
    public Guid? Guid { get; }
    public string Type { get; }
    public long Size { get; }
    public string FormattedSize { get; }
    public string Path { get; }
    public UserModel CreatedBy { get; }
    public DateTime CreatedOn { get; }

    public FileModel(FileAsset file, Memoizer? memo = null)
        : base(file)
    {
        Name = file.Name;
        Guid = file.Guid;
        Type = file.MediaType.ToString();
        Size = file.Length;
        FormattedSize = FileSize.Format(file.Length);
        Path = file.LinkPath;
        CreatedBy = file.User.Memo(memo, () => new UserModel(file.User, memo));
        CreatedOn = file.CreatedOn;
    }
}