﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;
using PlatoCore.Localization.Locales;

namespace PlatoCore.Localization.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoLocalization(this IServiceCollection services)
        {
            
            // Configure current culture
            services.Configure<LocaleOptions>(options =>
            {
                options.WatchForChanges = false;
                options.Culture = "en-US";
            });

            // Available time zones provider
            services.AddSingleton<ITimeZoneProvider, TimeZoneProvider>();

            // Local date time provider
            services.AddScoped<ILocalDateTimeProvider, LocalDateTimeProvider>();
            
            // Locales
            services.AddSingleton<ILocaleLocator, LocaleLocator>();
            services.AddSingleton<ILocaleCompositionStrategy, LocaleCompositionStrategy>();
            services.AddSingleton<ILocaleProvider, LocaleProvider>();
            services.AddSingleton<ILocaleWatcher, LocaleWatcher>();
            services.AddSingleton<ILocaleStore, LocaleStore>();
            
            return services;

        }

        public static void UsePlatoLocalization(this IApplicationBuilder app)
        {
            // Initialize locale directory watcher
            var watcher = app.ApplicationServices.GetRequiredService<ILocaleWatcher>();
            watcher.WatchForChanges();
        }

    }
}
