namespace Juniper.IO
{
    public class TypeFilteredCacheLayer<FilterTypeT> :
        AbstractTypeFilteredCacheLayer
    {
        public TypeFilteredCacheLayer(ICacheDestinationLayer destination)
            : base(destination)
        { }

        public override bool CanCache(ContentReference fileRef)
        {
            return fileRef is FilterTypeT
                && base.CanCache(fileRef);
        }
    }
}
