using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.EntityFrameworkCore;
using ACI.HAM.Api.BackgroundServices;
using ACI.HAM.Api.Infrastructure.Filters;
using Microsoft.Extensions.Hosting;
using ACI.HAM.Core.Extensions;
using ACI.HAM.Mail.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using ACI.HAM.Core.Profiles;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using coreLocalization = ACI.HAM.Core.Localization;
using mailLocalization = ACI.HAM.Mail.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ACI.HAM.Mail.Settings;
using ACI.HAM.Core.Localization.IdentityErrors;
using ACI.HAM.Api.Infrastructure.Configurations;
using ACI.HAM.Core.Data;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using ACI.HAM.Api.Services;
using ACI.HAM.Settings;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using ACI.HAM.Core.Infrastructure.Middlewares;

namespace ACI.HAM.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var certificate = new X509Certificate2(_configuration["AppSettings:CertificateFile"], @_configuration["AppSettings:CertificatePassword"]);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(_configuration["AppSettings:PersistKeysDirectory"]))
                .ProtectKeysWithCertificate(certificate);
#if DEBUG
            //var provider = services.BuildServiceProvider();
            //var dataProtector = provider.GetRequiredService<IDataProtectionProvider>().CreateProtector("AppSettings.ApiKeyProtector");
            //string encryptionKey = "w7G5v8J9k3L6m2N1p4Q7r8T5v2W9x1Y3";
            //string encryptedEncryptionKey = dataProtector.Protect(encryptionKey);
            //string encryptionKey2 = dataProtector.Unprotect(encryptedEncryptionKey);
#endif
            services.AddRouting(options => options.LowercaseUrls = true)
            .AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {                    
                builder.WithOrigins(_configuration["UI:BaseUrl"]).AllowAnyMethod().AllowAnyHeader();
            }))
            .AddMvcCore(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.Filters.Add<HttpGlobalExceptionFilter>();
                options.Filters.Add<ValidateModelStateFilter>();
            })
            .AddApiExplorer()
            .AddDataAnnotations()
            .AddDataAnnotationsLocalization(o =>
            {
                o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(coreLocalization.DataAnnotations));
                o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(mailLocalization.DataAnnotations));
            });
            services.AddPooledDbContextFactory<BaseContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MsSqlDb")), poolSize: 10);
            services.AddScoped<UserRepository>();
            services.AddScoped<BaseContextFactory>();
            services.AddScoped(serviceProvider =>
            {
                BaseContextFactory pooledContextFactory = serviceProvider.GetRequiredService<BaseContextFactory>();
                return pooledContextFactory.CreateDbContext();
            });
            services.AddScoped<DbInitializer>();
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<BaseContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");
            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(3));
            services.Configure<UISettings>(_configuration.GetSection("UI"));
            services.Configure<JwtSettings>(_configuration.GetSection("JwtSettings"));
            services.Configure<MailSettings>(_configuration.GetSection("MailSettings"));
            services.Configure<PingWebsiteSettings>(_configuration.GetSection("PingWebsite"));
            services.AddHostedService<PingWebsiteBackgroundService>();
            services.AddHttpClient(nameof(PingWebsiteBackgroundService));
            services.AddCoreComponents();
            services.AddMailComponents();
            services.AddFeatureManagement().AddFeatureFilter<TimeWindowFilter>();
            services.AddHealthChecks().AddSqlServer(_configuration.GetConnectionString("MsSqlDb"));
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version"),
                    new MediaTypeApiVersionReader("x-api-version"));

            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            var mapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.ApplyCurrentCultureToResponseHeaders = true;
                options.SupportedCultures = new List<CultureInfo> { new("en"), new("es") };
                options.SupportedUICultures = new List<CultureInfo> { new("en"), new("es") };
            });
            services.AddAuthorization();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["JwtSettings:ValidIssuer"],
                        ValidAudience = _configuration["JwtSettings:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]))
                    };
                });
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            _ = app.SeedDatabaseAsync();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.DocExpansion(DocExpansion.None);
                }
            });
            app.UseRequestLocalization();
            app.UseRouting();
            app.UseCors("ApiCorsPolicy");
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                });
            });
        }
    }
}
