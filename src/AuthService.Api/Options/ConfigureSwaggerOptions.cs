using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AuthService.Api.Options
{
    /// <summary>
    /// Configurando o Swagger
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

                var apiPath = Path.Combine(AppContext.BaseDirectory, "AuthService.Api.xml");
                var applicationPath = Path.Combine(AppContext.BaseDirectory, "AuthService.Application.xml");

                options.IncludeXmlComments(apiPath);
                options.IncludeXmlComments(applicationPath);
            }
            
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "AuthService API",
                Version = description.ApiVersion.ToString(),
                Description = "API responsável pela autenticação.",
                Contact = new OpenApiContact()
                {
                    Name = "André Magalhães",
                    Email = "andre.magalhaes02@gmail.com"
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " Essa versão foi descontinuada.";
            }

            return info;
        }
    }
}
