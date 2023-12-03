namespace Juniper.Caching;

public interface ICacheDestinationLayer : ICacheSourceLayer
{
    bool CanCache(ContentReference fileRef);

    Stream Create(ContentReference fileRef);

    bool Delete(ContentReference fileRef);
}
