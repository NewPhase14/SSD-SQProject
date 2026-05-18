using System.ComponentModel.DataAnnotations;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class TfaOptionsExtensions
{
    public static TfaOptions AddTfaOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var tfaOptions = new TfaOptions();
        configuration.GetSection("TfaOptions").Bind(tfaOptions);

        services.Configure<TfaOptions>(configuration.GetSection("TfaOptions"));

        ICollection<ValidationResult> results = new List<ValidationResult>();
        var validated = Validator.TryValidateObject(tfaOptions, new ValidationContext(tfaOptions), results, true);
        if (!validated)
            throw new Exception(
                $"Missing issuer, digits or period " +
                $"{string.Join(", ", results.Select(r => r.ErrorMessage))}");

        return tfaOptions;
    }
    
}