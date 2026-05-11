using System.ComponentModel.DataAnnotations;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class EncryptionExtensions
{
    public static Encryption AddEncryption(this IServiceCollection services, IConfiguration configuration)
    {
        var encryption = new Encryption();
        configuration.GetSection("Encryption").Bind(encryption);

        services.Configure<Encryption>(configuration.GetSection("Encryption"));

        ICollection<ValidationResult> results = new List<ValidationResult>();
        var validated = Validator.TryValidateObject(encryption, new ValidationContext(encryption), results, true);
        if (!validated)
            throw new Exception(
                $"Missing master key" +
                $"{string.Join(", ", results.Select(r => r.ErrorMessage))}");

        return encryption;
    }

}