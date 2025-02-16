using EasyWoWMacro.Business.Services;
using EasyWoWMacro.Business.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace EasyWoWMacro.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEasyWoWMacroBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IMacroParser, MacroParser>();

        return services;
    }
}