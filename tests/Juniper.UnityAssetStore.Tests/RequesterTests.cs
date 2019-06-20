using System;
using System.Threading.Tasks;

using Juniper.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.UnityAssetStore.Tests
{
    [TestClass]
    public class RequesterTests
    {
        private const string publisherID = "1";
        private const string categoryID = "6";
        private const string assetID = "82022";

        Requester req;

        [TestInitialize]
        public void Init()
        {
            req = new Requester(new Deserializer());
        }

        [TestMethod]
        public async Task GetCategories()
        {
            var categories = await req.GetCategories();
            foreach (var category in categories)
            {
                Console.WriteLine(category);
            }
        }

        [TestMethod]
        public async Task GetCategoryName()
        {
            var name = await req.GetCategoryName(categoryID);
        }



        [TestMethod]
        public async Task GetAssetSummary()
        {
            var summary = await req.GetAssetSummary(assetID);
        }


        [TestMethod]
        public async Task GetAssetDetails()
        {
            var details = await req.GetAssetDetails(assetID);
        }

        [TestMethod]
        public async Task GetAssetPrice()
        {
            var price = await req.GetAssetPrice(assetID);
        }

        [TestMethod]
        public async Task GetTopLatestAssets()
        {
            var assets = await req.GetTopLatestAssets(categoryID);
        }

        [TestMethod]
        public async Task GetTopGrossingAssets()
        {
            var assets = await req.GetTopGrossingAssets(categoryID);
        }

        [TestMethod]
        public async Task GetTopFreeAssets()
        {
            var assets = await req.GetTopFreeAssets(categoryID);
        }

        [TestMethod]
        public async Task GetTopPaidAssets()
        {
            var assets = await req.GetTopPaidAssets(categoryID);
        }

        [TestMethod]
        public async Task GetAssetContents()
        {
            var contents = await req.GetAssetContents(assetID);
        }

        [TestMethod]
        public async Task GetPublisherName()
        {
            var name = await req.GetPublisherName(publisherID);
        }

        [TestMethod]
        public async Task GetPublisherDetail()
        {
            var detail = await req.GetPublisherDetail(publisherID);
        }

        [TestMethod]
        public async Task GetCurrentSale()
        {
            var sale = await req.GetCurrentSale();
        }

        [TestMethod]
        public async Task SearchStore()
        {
            var assets = await req.Search(new StoreSearch()
                .Category(categoryID)
                .Page(3));
        }

        [TestMethod]
        public async Task GetDownloads()
        {
            var downloads = await req.GetDownloads("sean.mcbeth@gmail.com", "RzKuj0fd9f", "");
        }
    }
}
