using System;
using System.Security.Cryptography;
using System.Text;

namespace Juniper.Imaging
{

    public static class GoogleMaps
    {
        private static Uri Sign(Uri inputUri, string keyString)
        {
            var pkBytes = Convert.FromBase64String(keyString.FromGoogleModifiedBase64());
            var hasher = new HMACSHA1(pkBytes);

            var urlBytes = Encoding.ASCII.GetBytes(inputUri.LocalPath + inputUri.Query);
            var hash = hasher.ComputeHash(urlBytes);
            var signature = Convert.ToBase64String(hash).ToGoogleModifiedBase64();

            var outputUri = new UriBuilder(inputUri);
            outputUri.AddQuery("signature", signature);
            return outputUri.Uri;
        }

        static void Main()
        {

            // Note: Generally, you should store your private key someplace safe
            // and read them into your code

            const string keyString = "YOUR_PRIVATE_KEY";

            // The URL shown in these examples is a static URL which should already
            // be URL-encoded. In practice, you will likely have code
            // which assembles your URL from user or web service input
            // and plugs those values into its parameters.
            const string urlString = "YOUR_URL_TO_SIGN";

            Console.WriteLine("Enter the URL (must be URL-encoded) to sign: ");
            var inputUrl = Console.ReadLine();
            if (inputUrl.Length == 0)
            {
                inputUrl = urlString;
            }

            Console.WriteLine("Enter the Private key to sign the URL: ");
            var inputKey = Console.ReadLine();
            if (inputKey.Length == 0)
            {
                inputKey = keyString;
            }

            Console.WriteLine(Sign(new Uri(inputUrl), inputKey));
        }
    }
}