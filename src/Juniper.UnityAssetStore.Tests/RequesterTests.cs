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
        public async Task GetCategoriesAsync()
        {
            var categories = await req
                .GetCategoriesAsync()
                .ConfigureAwait(false);
            foreach (var category in categories)
            {
                Console.WriteLine(category);
            }
        }

        [TestMethod]
        public async Task GetCategoryNameAsync()
        {
            _ = await req
                .GetCategoryNameAsync(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetSummaryAsync()
        {
            _ = await req
                .GetAssetSummaryAsync(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetDetailsAsync()
        {
            _ = await req
                .GetAssetDetailsAsync(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetPriceAsync()
        {
            _ = await req
                .GetAssetPriceAsync(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopLatestAssetsAsync()
        {
            _ = await req
                .GetTopLatestAssetsAsync(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopGrossingAssetsAsync()
        {
            _ = await req
                .GetTopGrossingAssetsAsync(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopFreeAssetsAsync()
        {
            _ = await req
                .GetTopFreeAssetsAsync(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTopPaidAssetsAsync()
        {
            _ = await req
                .GetTopPaidAssetsAsync(categoryID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAssetContentsAsync()
        {
            _ = await req
                .GetAssetContentsAsync(assetID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPublisherNameAsync()
        {
            _ = await req
                .GetPublisherNameAsync(publisherID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPublisherDetailAsync()
        {
            _ = await req
                .GetPublisherDetailAsync(publisherID)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetCurrentSaleAsync()
        {
            _ = await req
                .GetCurrentSaleAsync()
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SearchStoreAsync()
        {
            _ = await req
                .SearchAsync(new StoreSearch()
                    .Category(categoryID)
                    .Page(3))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetDownloadsAsync()
        {
            _ = await Requester
                .GetDownloadsAsync("sean.mcbeth@gmail.com", "RzKuj0fd9f", "")
                .ConfigureAwait(false);
        }
    }
}