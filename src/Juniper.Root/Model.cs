namespace Juniper
{
    public partial class MediaType
    {
        public partial class Model : MediaType
        {
            private static List<Model> _allModel;
            private static List<Model> AllMod => _allModel ??= new();
            public static IReadOnlyCollection<Model> AllModel => AllMod;
            public static readonly Model AnyModel = new("*");

            public Model(string value, params string[] extensions) : base("model", value, extensions)
            {
                if (SubType != "*")
                {
                    AllMod.Add(this);
                }
            }

            public Model(MediaType copy)
                : this(
                      copy.Type == "model"
                      ? copy.FullSubType
                      : throw new ArgumentException("Invalid media type", nameof(copy)),
                      copy.Extensions.ToArray())
            {

            }
        }
    }
}
