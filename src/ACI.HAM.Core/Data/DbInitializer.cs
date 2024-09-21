using System.Threading.Tasks;
using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Data
{
    public class DbInitializer
    {
        public async static Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            IdentityResult administratorRole;
            const string ADMINISTRATOR_ROLE = "Administrator";
            if (!await roleManager.RoleExistsAsync(ADMINISTRATOR_ROLE))
            {
                administratorRole = await roleManager.CreateAsync(new Role(ADMINISTRATOR_ROLE));
            }
            IdentityResult companyAdministratorRole;
            const string COMPANY_ADMINISTRATOR_ROLE = "Company administrator";
            if (!await roleManager.RoleExistsAsync(COMPANY_ADMINISTRATOR_ROLE))
            {
                companyAdministratorRole = await roleManager.CreateAsync(new Role(COMPANY_ADMINISTRATOR_ROLE));
            }
            IdentityResult basicRole;
            const string BASIC_ROLE = "Basic";
            if (!await roleManager.RoleExistsAsync(BASIC_ROLE))
            {
                basicRole = await roleManager.CreateAsync(new Role(BASIC_ROLE));
            }
            const string EMAIL_ADMINISTRATOR = "paugonzalez@acigrup.com";
            var administratorUser = await userManager.FindByNameAsync(EMAIL_ADMINISTRATOR);
            if (administratorUser == null)
            {
                var passwordHasher = new PasswordHasher<User>();
                administratorUser = new User
                {
                    CreateDate = DateTimeOffset.UtcNow,
                    UserName = EMAIL_ADMINISTRATOR,
                    Email = EMAIL_ADMINISTRATOR,
                    Firstname = "Pablo",
                    Lastname = "González",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "l56Ba0")                    
                };
                await userManager.CreateAsync(administratorUser);
            }
            if (!await userManager.IsInRoleAsync(administratorUser, ADMINISTRATOR_ROLE))
            {
                await userManager.AddToRoleAsync(administratorUser, ADMINISTRATOR_ROLE);
            }
        }
    }
}
