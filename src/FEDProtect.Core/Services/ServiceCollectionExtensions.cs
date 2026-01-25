using Microsoft.Extensions.DependencyInjection;

namespace FEDProtect.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFEDProtectServices(this IServiceCollection services, string? profilesDirectory = null)
    {
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<IProfileManager>(sp => new ProfileManager(profilesDirectory, sp.GetService<ILogger>()));
        services.AddSingleton<IHardeningApplier, HardeningApplier>();
        services.AddSingleton<IOsChecker, OsChecker>();
        services.AddSingleton<IOutputFormatter, OutputFormatter>();
        
        return services;
    }
}
