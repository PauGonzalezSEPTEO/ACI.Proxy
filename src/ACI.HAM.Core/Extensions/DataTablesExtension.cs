using AutoMapper;
using ACI.HAM.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ACI.HAM.Core.Extensions
{
    public static class DataTablesExtension
    {
        public static async Task<DataTablesResult<U>> GetDataTablesResultAsync<T, U>(this IQueryable<T> query, IMapper mapper, DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
            where T : IFilterDto<T, U>
            where U : class
        {
            string ordering = string.Empty;
            if (dataTablesParameters.Order != null)
            {
                foreach (var order in dataTablesParameters.Order)
                {
                    string orderBy = dataTablesParameters.Columns[order.Column].Data;
                    string ascending = order.Dir == DataTablesOrderDir.Asc ? "asc" : "desc";
                    if (string.IsNullOrEmpty(ordering))
                    {
                        ordering = string.Format("{0} {1}", dataTablesParameters.Columns[order.Column].Data, order.Dir == DataTablesOrderDir.Asc ? "asc" : "desc");
                    }
                    else
                    {
                        ordering += string.Format(" {0} {1}", dataTablesParameters.Columns[order.Column].Data, order.Dir == DataTablesOrderDir.Asc ? "asc" : "desc");
                    }
                }
            }
            string search = dataTablesParameters.Search?.Value;
            MethodInfo method = typeof(T).GetMethod(nameof(IFilterDto<T, U>.FilterAndOrder));
            object[] parameters = new object[] { query, mapper, search, ordering, languageCode };
            IQueryable<U> queryFilteredAndOrdered = (IQueryable<U>)method.Invoke(null, parameters);
            return new DataTablesResult<U>()
            {
                Data = await queryFilteredAndOrdered.Skip(dataTablesParameters.Start).Take(dataTablesParameters.Length).ToListAsync(cancellationToken),
                Draw = dataTablesParameters.Draw,
                RecordsTotal = query.Count(),
                RecordsFiltered = queryFilteredAndOrdered.Count()
            };
        }
    }
}
