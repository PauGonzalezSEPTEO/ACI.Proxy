using System.Linq;
using AutoMapper;

namespace ACI.Proxy.Core.Dtos
{
    public interface IFilterDto<T, U>
    {
        static IQueryable<U> FilterAndOrder(IQueryable<T> query, IMapper mapper, string search, string languageCode = null) => throw new NotImplementedException();
    }
}
