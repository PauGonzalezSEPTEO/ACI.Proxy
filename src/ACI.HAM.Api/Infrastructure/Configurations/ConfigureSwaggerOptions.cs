using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;

namespace ACI.HAM.Api.Infrastructure.Configurations
{
    public class ConfigureSwaggerOptions
        : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configure each API discovered for Swagger Documentation
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            // add swagger document for every API version discovered
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
                options.OrderActionsBy(x => x.RelativePath);
                options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ACI.HAM.Api.xml"));
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. <br/><br/> 
                      Enter 'Bearer' [space] and then your access token in the text input below.
                      <br/><br/>Example: 'Bearer 12345abcdef'",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Name = HeaderNames.Authorization,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
        }

        /// <summary>
        /// Configure Swagger Options. Inherited from the Interface
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// Create information about the version of the API
        /// </summary>
        /// <param name="desc"></param>
        /// <returns>Information about the API</returns>
        private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
        {
            var info = new OpenApiInfo()
            {
                Title = string.Format("ACI.HAM.Api.{0} Api", desc.ApiVersion),
                Version = desc.ApiVersion.ToString(),
                Contact = new OpenApiContact
                {
                    Name = "Pablo González",
                    Url = new Uri("https://acigrup.com/"),
                }
            };
            if (desc.IsDeprecated)
            {
                info.Description += " This Api version has been deprecated. Please use one of the new Apis available from the explorer.";
            }
            return info;
        }
    }
}
