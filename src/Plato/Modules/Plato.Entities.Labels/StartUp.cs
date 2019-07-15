﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Entities.Labels.Search;
using Plato.Entities.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores;
using Plato.Internal.Stores.Abstractions.FederatedQueries;

namespace Plato.Entities.Labels
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Federated search
            services.AddScoped<IFederatedQueryManager<Entity>, FederatedQueryManager<Entity>>();
            services.AddScoped<IFederatedQueryProvider<Entity>, EntityQueries<Entity>>();

            services.AddScoped<IFederatedQueryManager<FeatureEntityCount>, FederatedQueryManager<FeatureEntityCount>>();
            services.AddScoped<IFederatedQueryProvider<FeatureEntityCount>, FeatureEntityCountQueries<FeatureEntityCount>>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}