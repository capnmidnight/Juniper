using Juniper.IO;
using Juniper.Progress;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Juniper.World.GIS.Google;

public class GoogleMapsStreamingClientOptions {
    public const string Google = "Google";
    public string? APIKey { get; set; }
    public string? SigningKey { get; set; }
}


public static class GoogleMapsStreamingClientConfiguration
{
    public static WebApplicationBuilder ConfigureJuniperGoogleMaps(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<GoogleMapsStreamingClientOptions>(builder.Configuration.GetSection(GoogleMapsStreamingClientOptions.Google));

        builder.Services.AddSingleton<GoogleMapsStreamingClient>();
        builder.Services.AddSingleton<IGoogleMapsStreamingClient>(serviceProvider =>
            serviceProvider.GetRequiredService<GoogleMapsStreamingClient>());
        return builder;
    }
}
public class GoogleMapsStreamingClient : IGoogleMapsStreamingClient
{
    protected HttpClient Http { get; private set; }
    protected string ApiKey { get; private set; }
    protected string SigningKey { get; private set; }
    protected CachingStrategy Cache { get; private set; }

    private Exception? lastError;

    public GoogleMapsStreamingClient(HttpClient http, IOptions<GoogleMapsStreamingClientOptions> options)
        : this(http,
              options.Value.APIKey ?? throw new ArgumentNullException(nameof(GoogleMapsStreamingClientOptions.APIKey)),
              options.Value.SigningKey ?? throw new ArgumentNullException(nameof(GoogleMapsStreamingClientOptions.SigningKey)))
    {
    }

    public GoogleMapsStreamingClient(HttpClient http, string apiKey, string signingKey)
    {
        Http = http;
        ApiKey = apiKey;
        SigningKey = signingKey;
        Cache = CachingStrategy.GetNoCache();
    }

    public string Status => lastError?.Message ?? "NONE";

    public void ClearError()
    {
        lastError = null;
    }

    public Task<Stream?> ReverseGeocodeStreamAsync(LatLngPoint latLng, IProgress? prog = null)
    {
        return Cache.GetStreamAsync(new ReverseGeocodingRequest(Http, ApiKey)
        {
            Location = latLng
        }, prog);
    }

    public Task<Stream?> GetMetadataStreamAsync(string pano, int searchRadius = 50, IProgress? prog = null)
    {
        return Cache.GetStreamAsync(new MetadataRequest(Http, ApiKey, SigningKey)
        {
            Pano = pano,
            Radius = searchRadius
        }, prog);
    }

    public Task<Stream?> SearchMetadataStreamAsync(string placeName, int searchRadius = 50, IProgress? prog = null)
    {
        return Cache.GetStreamAsync(new MetadataRequest(Http, ApiKey, SigningKey)
        {
            Place = placeName,
            Radius = searchRadius
        }, prog);
    }

    public Task<Stream?> GetMetadataStreamAsync(LatLngPoint latLng, int searchRadius = 50, IProgress? prog = null)
    {
        return Cache.GetStreamAsync(new MetadataRequest(Http, ApiKey, SigningKey)
        {
            Location = latLng,
            Radius = searchRadius
        }, prog);
    }

    public async Task<Stream?> SearchMetadataStreamAsync(string? searchLocation, string? searchPano, LatLngPoint? searchPoint, int searchRadius, IProgress? prog = null)
    {
        try
        {
            var metaSubProgs = prog.Split("Searching by Pano_ID", "Searching by Lat/Lng", "Searching by Location Name");
            if (searchPano != null)
            {
                return await GetMetadataStreamAsync(searchPano, searchRadius, metaSubProgs[0])
                    .ConfigureAwait(false);
            }

            if (searchPoint is not null)
            {
                return await GetMetadataStreamAsync(searchPoint, searchRadius, metaSubProgs[1])
                    .ConfigureAwait(false);
            }

            if (searchLocation != null)
            {
                return await SearchMetadataStreamAsync(searchLocation, searchRadius, metaSubProgs[2])
                    .ConfigureAwait(false);
            }

            return default;
        }
        finally
        {
            prog?.Report(1);
        }
    }

    public virtual Task<Stream?> GetImageStreamAsync(string pano, int fovDegrees, int headingDegrees, int pitchDegrees, IProgress? prog = null)
    {
        return Cache.GetStreamAsync(new ImageRequest(Http, ApiKey, SigningKey, new Size(640, 640))
        {
            Pano = pano,
            FOVDegrees = fovDegrees,
            HeadingDegrees = headingDegrees,
            PitchDegrees = pitchDegrees
        }, prog);
    }
}
