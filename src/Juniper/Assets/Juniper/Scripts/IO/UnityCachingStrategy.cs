namespace Juniper.IO
{
    public class UnityCachingStrategy : CachingStrategy
    {
        public UnityCachingStrategy()
        {
            AddLayer(new StreamingAssetsCacheLayer());
        }
    }
}
