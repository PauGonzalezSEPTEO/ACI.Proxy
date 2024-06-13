using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace ACI.Base.Core.Dtos
{
    public interface IFilterDto<T, U>
    {
        static IQueryable<U> FilterAndOrder(IQueryable<T> query, IMapper mapper, string search, string languageCode = null) => throw new NotImplementedException();
    }
}
