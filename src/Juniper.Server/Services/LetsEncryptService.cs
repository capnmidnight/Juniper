using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

using System.Security.Cryptography.X509Certificates;

namespace Juniper.Services
{
    public class LetsEncryptService : IConfigureOptions<KestrelServerOptions>
    {
        private readonly IConfiguration config;
        private readonly ILogger<LetsEncryptService> logger;

        public LetsEncryptService(IConfiguration config, ILogger<LetsEncryptService> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public void Configure(KestrelServerOptions options)
        {
            options.AddServerHeader = false;

            var letsEncrypt = config.GetSection("LetsEncrypt");
            var fullChainFileName = letsEncrypt.GetValue<string>("FullChainPath");
            var privKeyFileName = letsEncrypt.GetValue<string>("PrivateKeyPath");
            if (!File.Exists(fullChainFileName))
            {
                logger.LogError("Certificate file {0} does not exist.", fullChainFileName);
            }
            else if (!File.Exists(privKeyFileName))
            {
                logger.LogError("Private key file {0} does not exist.", privKeyFileName);
            }
            else
            {
                try
                {
                    var fullChain = X509Certificate2.CreateFromPemFile(fullChainFileName, privKeyFileName);
                    options.ConfigureHttpsDefaults(o =>
                    {
                        o.ServerCertificate = fullChain;
                    });
                    logger.LogInformation("TLS Certificate loaded");
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "Certifcate error");
                }
            }
        }
    }
}
