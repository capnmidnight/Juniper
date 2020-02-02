namespace Juniper.IO
{
    public class TypeFilteredCacheLayer<FilterTypeT1, FilterTypeT2> :
        AbstractTypeFilteredCacheLayer
    {
        public TypeFilteredCacheLayer(ICacheDestinationLayer destination)
            : base(destination)
        { }

        public override bool CanCache(ContentReference fileRef)
        {
            return (fileRef is FilterTypeT1
                    || fileRef is FilterTypeT2)
                && base.CanCache(fileRef);
        }
    }
}
