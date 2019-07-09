using System;
using System.Security.Cryptography;
using System.Text;

namespace Juniper.GoogleMaps
{

    public static class Requester
    {
        public static string Sign(string url, string keyString)
        {
            // converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
            var usablePrivateKey = keyString.Replace("-", "+").Replace("_", "/");
            var privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

            var uri = new Uri(url);
            var encodedPathAndQueryBytes = Encoding.ASCII.GetBytes(uri.LocalPath + uri.Query);

            // compute the hash
            var algorithm = new HMACSHA1(privateKeyBytes);
            var hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

            // convert the bytes to string and make url-safe by replacing '+' and '/' characters
            var signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

            // Add the signature to the existing URI.
            return $"{uri.Scheme}://{uri.Host}{uri.LocalPath}{uri.Query}&signature={signature}";
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

            string inputUrl = null;
            string inputKey = null;

            Console.WriteLine("Enter the URL (must be URL-encoded) to sign: ");
            inputUrl = Console.ReadLine();
            if (inputUrl.Length == 0)
            {
                inputUrl = urlString;
            }

            Console.WriteLine("Enter the Private key to sign the URL: ");
            inputKey = Console.ReadLine();
            if (inputKey.Length == 0)
            {
                inputKey = keyString;
            }

            Console.WriteLine(Sign(inputUrl, inputKey));
        }
    }
}