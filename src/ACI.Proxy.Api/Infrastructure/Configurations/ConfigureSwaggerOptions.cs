using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Linq;

namespace ACI.Proxy.Api.Infrastructure.Configurations
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (context.DocumentName == "Public Api")
            {
                swaggerDoc.Components.SecuritySchemes.Remove("Bearer");
            }
        }
    }

    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();
            if (hasAllowAnonymous)
            {
                return;
            }
            var apiDescription = context.ApiDescription;
            var groupName = apiDescription.GroupName;
            var parameters = operation.Parameters ?? new List<OpenApiParameter>();
            switch (groupName)
            {
                case "Public Api":
                    parameters.Add(new OpenApiParameter
                    {
                        Name = "x-api-key",
                        In = ParameterLocation.Header,
                        Description = "API key needed to access the endpoints.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    });
                    parameters.Add(new OpenApiParameter
                    {
                        Name = "email",
                        In = ParameterLocation.Header,
                        Description = "Email header for authentication.",
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    });
                    break;
                case "Private Api":
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            }, new string[0]
                        }
                    });
                    break;
            }
            operation.Parameters = parameters;
        }
    }

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
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }
            options.OrderActionsBy(x => x.RelativePath);
            options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ACI.Proxy.Api.xml"));
            options.OperationFilter<SwaggerOperationFilter>();
            options.DocumentFilter<SwaggerDocumentFilter>();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Use bearer token to authorize",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
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
                Title = string.Format("ACI.Proxy.Api", desc.ApiVersion),
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
