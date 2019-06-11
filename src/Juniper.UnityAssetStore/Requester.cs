using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.UnityAssetStore
{
    public class Package
    {
        public string package_version_id;
    }

    public class Asset
    {

    }

    public class Category
    {

    }

    public class Publisher
    {

    }

    public class Requester
    {
        const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        const string UnityAssetStoreServiceRoot = "https://www.assetstore.unity3d.com/api/en-US/";

        private static async Task<List<T>> GetList<T>(string url)
        {
            var response = await new HTTP.Requester(UnityAssetStoreServiceRoot + url)
                .Header("X-Unity-Session", UnityAssetStoreToken)
                .Get();

            if (response.Status == System.Net.HttpStatusCode.OK
                && response.Value != null)
            {
                var text = response.Value?.ReadString();
                System.Console.WriteLine(text);
            }

            return null;
        }

        private static async Task<T> Get<T>(string url)
        {
            var list = await GetList<T>(url);
            if (list == null)
            {
                return default;
            }
            else
            {
                return list.FirstOrDefault();
            }
        }

        public static async Task<List<Package>> SearchPackages(string query, int limit)
        {
            return await GetList<Package>($"search/xplr/search.json?query=${query}&limit=${limit}");
        }

        public static async Task<Package> GetPackageOverview(string packageID)
        {
            return await Get<Package>($"content/overview/{packageID}.json");
        }

        public static async Task<List<Asset>> GetPackageAssets(string packageID, string version)
        {
            return await GetList<Asset>($"content/assets/{packageID}/{version}.json");
        }

        public static async Task<List<Asset>> GetPackageAssets(string packageID)
        {
            var package = await GetPackageOverview(packageID);
            return await GetPackageAssets(packageID, package.package_version_id);
        }

        public static async Task<List<Category>> GetCategories()
        {
            return await GetList<Category>("home/categories.json");
        }

        public static async Task<Category> GetCategory(string categoryID)
        {
            return await Get<Category>($"head/category/{categoryID}.json");
        }

        public static async Task<List<Package>> GetPackagesInCategory(string categoryID)
        {
            return await GetList<Package>($"category/results/{categoryID}.json");
        }

        public static async Task<Publisher> GetPublisher(string publisherID)
        {
            return await Get<Publisher>($"publisher/overview/{publisherID}.json");
        }

        public static async Task<List<Package>> GetPackagesFromPublisher(string publisherID)
        {
            return await GetList<Package>($"publisher/results/{publisherID}.json");
        }
    }
}
