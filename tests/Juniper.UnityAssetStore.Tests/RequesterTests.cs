using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.UnityAssetStore.Tests
{
    [Ignore]
    [TestClass]
    public class RequesterTests
    {
        private const string publisherID = "1";
        private const string categoryID = "6";
        private const string assetID = "82022";

        private Requester req;

        [TestInitialize]
        public void Init()
        {
            req = new Requester();
        }

        [TestMethod]
        public async Task GetCategories()
        {
            var categories = await req
                .GetCategories()
                .ConfigureAwait(false);
            foreach (var category in categories)
            {
                Console.WriteLine(category);
            }
        }

        [TestMethod]
        public async Task GetCategoryName()
        {
            _ = await req
                .GetCategoryName(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetSummary()
        {
            _ = await req
                .GetAssetSummary(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetDetails()
        {
            _ = await req
                .GetAssetDetails(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetPrice()
        {
            _ = await req
                .GetAssetPrice(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopLatestAssets()
        {
            _ = await req
                .GetTopLatestAssets(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopGrossingAssets()
        {
            _ = await req
                .GetTopGrossingAssets(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopFreeAssets()
        {
            _ = await req
                .GetTopFreeAssets(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopPaidAssets()
        {
            _ = await req
                .GetTopPaidAssets(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetContents()
        {
            _ = await req
                .GetAssetContents(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPublisherName()
        {
            _ = await req
                .GetPublisherName(publisherID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPublisherDetail()
        {
            _ = await req
                .GetPublisherDetail(publisherID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetCurrentSale()
        {
            _ = await req
                .GetCurrentSale()
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SearchStore()
        {
            _ = await req
                .Search(new StoreSearch()
                    .Category(categoryID)
                    .Page(3))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetDownloads()
        {
            _ = await req
                .GetDownloads("sean.mcbeth@gmail.com", "RzKuj0fd9f", "")
                .ConfigureAwait(false);
        }
    }
}