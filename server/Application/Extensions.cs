using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Services;
using Application.Validators.Conversations;
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
        services.AddScoped<IServiceLogic, ServiceLogic>();
        services.AddScoped<IListingService, ListingService>();
        
        
        //Validatiors
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateConversationRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SendMessageRequestValidator>();
        
        return services;
    }
}