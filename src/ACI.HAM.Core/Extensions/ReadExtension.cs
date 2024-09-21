using AutoMapper;
using ACI.HAM.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ACI.HAM.Core.Extensions
{
    public static class ReadExtension
    {
        public static async Task<List<U>> GetResultAsync<T, U>(this IQueryable<T> query, IMapper mapper, string search, string ordering, string languageCode = null, CancellationToken cancellationToken = default)
            where T : IFilterDto<T, U>
            where U : class
        {
            if (string.IsNullOrEmpty(ordering))
            {
                ordering = "name asc";
            }
            MethodInfo method = typeof(T).GetMethod(nameof(IFilterDto<T, U>.FilterAndOrder));
            object[] parameters = new object[] { query, mapper, search, ordering, languageCode };
            IQueryable<U> queryFilteredAndOrdered = (IQueryable<U>)method.Invoke(null, parameters);
            return await queryFilteredAndOrdered.ToListAsync();
        }
    }
}
