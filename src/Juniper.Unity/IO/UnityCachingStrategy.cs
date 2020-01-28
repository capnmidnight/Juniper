namespace Juniper.IO
{
    public class UnityCachingStrategy : CachingStrategy
    {
        public UnityCachingStrategy()
        {
            AppendLayer(new StreamingAssetsCacheLayer());
        }
    }
}
