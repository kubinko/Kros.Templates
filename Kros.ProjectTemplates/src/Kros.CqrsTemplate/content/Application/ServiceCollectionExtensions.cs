﻿using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Kros.KORM.Extensions.Asp;
using System.Reflection;
using MediatR;
using Kros.MediatR.Extensions;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for registering services for this project to the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register fluent validation.
        /// </summary>
        /// <param name="builder">MVC builder.</param>
        /// <returns>MVC builder.</returns>
        public static IMvcCoreBuilder AddFluentValidation(this IMvcCoreBuilder builder)
            => builder.AddFluentValidation(o =>
            {
                o.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                o.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });

        /// <summary>
        /// Add KORM database.
        /// </summary>
        /// <param name="services">DI container.</param>
        /// <param name="configuration">Configuration.</param>
        public static void AddKormDatabase(this IServiceCollection services, IConfiguration configuration)
            => services.AddKorm(configuration)
                .InitDatabaseForIdGenerator()
                .AddKormMigrations(configuration, o =>
                {
                    o.AddAssemblyScriptsProvider(Assembly.GetEntryAssembly(), "Kros.CqrsTemplate.SqlScripts");
                })
                .Migrate();

        /// <summary>
        /// Add MediatR.
        /// </summary>
        /// <param name="services">DI container.</param>
        public static IServiceCollection AddMediatRDependencies(this IServiceCollection services)
            => services.AddMediatR(Assembly.GetExecutingAssembly())
                .AddMediatRNullCheckPostProcessor();

        /// <summary>
        /// Add MediatR.
        /// </summary>
        /// <param name="services">DI container.</param>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
            => services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "Kros.CqrsTemplate.xml");

                if (File.Exists(filePath))
                {
                    c.IncludeXmlComments(filePath);
                }
                c.AddFluentValidationRules();
            });
    }
}