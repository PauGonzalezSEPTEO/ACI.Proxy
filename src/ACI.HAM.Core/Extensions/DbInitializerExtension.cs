using System.Threading.Tasks;
using ACI.HAM.Core.Data;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.HAM.Core.Extensions
{
    public static class DbInitializerExtension
    {
        public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                UserManager<User> userManager = services.GetRequiredService<UserManager<User>>();
                RoleManager<Role> roleManager = services.GetRequiredService<RoleManager<Role>>();
                await DbInitializer.InitializeAsync(userManager, roleManager);
            }
            return app;
        }
    }
}
