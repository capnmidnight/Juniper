namespace Juniper;

public partial class MediaType
{
    public static readonly MediaType Any = new("*", "*");
    private static MediaType[]? _all;
    public static IReadOnlyCollection<MediaType> All
    {
        get
        {
            if (_all is null)
            {
                _all = Application.AllApplication
                        .Cast<MediaType>()
                        .Union(Audio.AllAudio)
                        .Union(Chemical.AllChemical)
                        .Union(Font.AllFont)
                        .Union(Image.AllImage)
                        .Union(Message.AllMessage)
                        .Union(Model.AllModel)
                        .Union(Multipart.AllMultipart)
                        .Union(Text.AllText)
                        .Union(Video.AllVideo)
                        .Union(XConference.AllXConference)
                        .Union(XShader.AllXShader)
                        .ToArray();
            }

            return _all;
        }
    }
}
