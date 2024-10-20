using ACI.HAM.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.HAM.Core.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanCreateUser", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanDeleteUser", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanUpdateUser", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanCreateRole", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanDeleteRole", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanUpdateRole", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanCreateCompany", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanDeleteCompany", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanUpdateCompany", policy =>
                    policy.RequireRole("Administrator"));
                options.AddPolicy("CanCreateHotel", policy =>
                    policy.RequireRole("Administrator", "Company administrator"));
                options.AddPolicy("CanDeleteHotel", policy =>
                    policy.RequireRole("Administrator", "Company administrator"));
                options.AddPolicy("CanUpdateHotel", policy =>
                    policy.RequireRole("Administrator", "Company administrator"));
            });
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
