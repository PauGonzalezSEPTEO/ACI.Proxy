using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.EntityFrameworkCore;
using ACI.Proxy.Api.BackgroundServices;
using ACI.Proxy.Api.Infrastructure.Filters;
using Microsoft.Extensions.Hosting;
using ACI.Proxy.Core.Extensions;
using ACI.Proxy.Mail.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using ACI.Proxy.Core.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using ACI.Proxy.Core.Profiles;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using coreLocalization = ACI.Proxy.Core.Localization;
using mailLocalization = ACI.Proxy.Mail.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ACI.Proxy.Mail.Settings;
using ACI.Proxy.Core.Localization.IdentityErrors;
using ACI.Proxy.Api.Infrastructure.Configurations;
using ACI.Proxy.Core.Data;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using ACI.Proxy.Api.Services;
using ACI.Proxy.Settings;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using ACI.Proxy.Core.Infrastructure.Middlewares;
using ACI.Proxy.Api.Infrastructure;
using AspNetCoreRateLimit;
using ACI.Proxy.Core.Services;
#if DEBUG && ENCRYPT
using System.Diagnostics;
#endif

namespace ACI.Proxy.Api
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
            ApiKeySettings apiKeyConfiguration = _configuration.GetSection("ApiKey").Get<ApiKeySettings>();
#if (DEBUG && ENCRYPT) || TEST_ENCRYPT
            string certificateFile = Path.Combine(AppContext.BaseDirectory, "certificates", "certificate.pfx");
            string persistKeysDirectory = Path.Combine(AppContext.BaseDirectory, "certificates");
#else
            string certificateFile = apiKeyConfiguration.CertificateFile;
            string persistKeysDirectory = apiKeyConfiguration.PersistKeysDirectory;
#endif
            var certificate = new X509Certificate2(certificateFile, @apiKeyConfiguration.CertificatePassword);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(persistKeysDirectory))
                .ProtectKeysWithCertificate(certificate);
            services.AddSingleton<DataProtectorFactory>();
            services.AddSingleton(serviceProvider =>
            {
                var dataProtectionProvider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
                var dataProtectorFactory = new DataProtectorFactory(dataProtectionProvider);
                return dataProtectorFactory.CreateProtector("ApiKey.ApiKeyProtector");
            });
#if !(DEBUG && ENCRYPT)
            services.Configure<ClientRateLimitOptions>(_configuration.GetSection("RateLimiting"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddRouting(options => options.LowercaseUrls = true)
            .AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {                    
                builder.WithOrigins(_configuration.GetValue<string>("UI:BaseUrl")).AllowAnyMethod().AllowAnyHeader();
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
            services.AddPooledDbContextFactory<BaseContext>((serviceProvider, options) =>
            {
                var dataProtector = serviceProvider.GetRequiredService<IDataProtector>();
                string msSqlDb = dataProtector.Unprotect(_configuration.GetConnectionString("MsSqlDb"));
                options.UseSqlServer(msSqlDb);
            }, poolSize: 10);
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
            services.Configure<JwtSettings>(_configuration.GetSection("Jwt"));
            services.Configure<MailSettings>(_configuration.GetSection("Mail"));
            services.Configure<ApiKeySettings>(_configuration.GetSection("ApiKey"));
            services.Configure<PingWebsiteSettings>(_configuration.GetSection("PingWebsite"));
            services.AddMailComponents();
            services.AddCoreComponents();
            services.AddHostedService<PingWebsiteBackgroundService>();
            services.AddHostedService<UserApiKeyRotationBackgroundService>();
            services.AddHttpClient(nameof(PingWebsiteBackgroundService));
            services.AddFeatureManagement().AddFeatureFilter<TimeWindowFilter>();
            services.AddHealthChecks().AddSqlServer(serviceProvider =>
            {
                var dataProtector = serviceProvider.GetRequiredService<IDataProtector>();
                string msSqlDb = dataProtector.Unprotect(_configuration.GetConnectionString("MsSqlDb"));
                return msSqlDb;
            });
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
            JwtSettings jwtSettingsConfiguration = _configuration.GetSection("Jwt").Get<JwtSettings>();
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
                    ValidIssuer = jwtSettingsConfiguration.ValidIssuer,
                    ValidAudience = jwtSettingsConfiguration.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettingsConfiguration.SecurityKey))
                };
            });
            services.AddAuthorization();
#endif
        }

#if DEBUG && ENCRYPT
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDataProtectionProvider dataProtectionProvider, IServiceProvider serviceProvider)
#else
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IDataProtectionProvider dataProtectionProvider, IServiceProvider serviceProvider)
#endif
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            
#if DEBUG && ENCRYPT
            var dataProtector = serviceProvider.GetRequiredService<IDataProtector>();
            string hMACKey = "TEZNK3Ii7U0Gw0RkNxR/fUnfkS3oUZaEVL6cxdQLegI=";
            string encryptedHMACKey = dataProtector.Protect(hMACKey);
            string hMACKey2 = dataProtector.Unprotect(encryptedHMACKey);
            Debug.WriteLine($"encryptedHMACKey: {encryptedHMACKey}");
            string encryptionKey = "w7G5v8J9k3L6m2N1p4Q7r8T5v2W9x1Y3";
            string encryptedEncryptionKey = dataProtector.Protect(encryptionKey);
            string encryptionKey2 = dataProtector.Unprotect(encryptedEncryptionKey);
            Debug.WriteLine($"encryptedEncryptionKey: {encryptedEncryptionKey}");
            string msSqlDb = "Data Source=mssql,1433;Initial Catalog=Proxy;User ID=sa;Password=.acisa159753;MultipleActiveResultSets=true;TrustServerCertificate=true;";
            string encryptedMsSqlDb = dataProtector.Protect(msSqlDb);
            string msSqlDb2 = dataProtector.Unprotect(encryptedMsSqlDb);
            Debug.WriteLine($"encryptedMsSqlDb: {encryptedMsSqlDb}");
            string securityKey = "ASDFGHJKLqtfaaftfztfzljkjmkjhugyftyftdxrfxxthdtryjtrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrcccccccccccdxdd";
            string encryptedSecurityKey = dataProtector.Protect(securityKey);
            string securityKey2 = dataProtector.Unprotect(encryptedSecurityKey);
            Debug.WriteLine($"encryptedSecurityKey: {encryptedSecurityKey}");
            string mailPassword = "vECSnn8TYtavTK4AG6";
            string encryptedMailPassword = dataProtector.Protect(mailPassword);
            string mailPassword2 = dataProtector.Unprotect(encryptedMailPassword);
            Debug.WriteLine($"mailPassword: {encryptedMailPassword}");
#else
            string encryptedHMACKey = _configuration.GetValue<string>("ApiKey:HMACKey");            
            ApiKeyExtension.Initialize(dataProtectionProvider, encryptedHMACKey);
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
            app.UseClientRateLimiting();
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
#endif
        }
    }
}
