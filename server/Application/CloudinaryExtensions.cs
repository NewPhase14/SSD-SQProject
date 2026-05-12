using Application.Models;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application;

public static class CloudinaryExtensions
{
    public static CloudinaryOptions AddCloudinary(this IServiceCollection services,
        IConfiguration configuration)
    {
        var cloudinaryOptions = new CloudinaryOptions();
        configuration.GetSection("CloudinaryOptions").Bind(cloudinaryOptions);

        services.Configure<CloudinaryOptions>(configuration.GetSection("CloudinaryOptions"));

        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CloudinaryOptions>>().Value;
            var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
            return new Cloudinary(account);
        });

        return cloudinaryOptions;
    }
}