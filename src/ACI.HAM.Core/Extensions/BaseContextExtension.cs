using ACI.HAM.Core.Data;
using ACI.HAM.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ACI.HAM.Core.Extensions
{
    public static class BaseContextExtension
    {
        public static async Task<List<int>> GetUserCompaniesAsync(this BaseContext context, ClaimsPrincipal claimsPrincipal)
        {
            List<int> companies = new List<int>();
            if (claimsPrincipal != null)
            {
                var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = await context.Users
                    .Include(x => x.Companies)
                    .FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                {
                    companies = user.Companies.Select(x => x.CompanyId).ToList();
                }
            }
            return companies;
        }
    }
}
