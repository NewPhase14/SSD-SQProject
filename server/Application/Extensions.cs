using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Services;
using Application.Validators.Auth;
using Application.Validators.Conversations;
using Application.Validators.Listings;
using Application.Validators.Messages;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Extensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<ICryptoService, CryptoService>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<ICloudinaryImageService, CloudinaryImageImageService>();
        
        
        //Validators
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<ConversationCreateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<MessageSendRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ListingCreateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ListingUpdateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<AuthRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        
        return services;
    }
}