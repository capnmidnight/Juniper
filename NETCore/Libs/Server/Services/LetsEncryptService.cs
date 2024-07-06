using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Security.Cryptography.X509Certificates;

namespace Juniper.Services;

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

        var fullChainFileName = config.GetValue<string>("LetsEncrypt:FullChainPath");
        var privKeyFileName = config.GetValue<string>("LetsEncrypt:PrivateKeyPath");
        if (!File.Exists(fullChainFileName))
        {
            logger.LogError("Certificate file {fullChainFileName} does not exist.", fullChainFileName);
        }
        else if (!File.Exists(privKeyFileName))
        {
            logger.LogError("Private key file {privKeyFileName} does not exist.", privKeyFileName);
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
