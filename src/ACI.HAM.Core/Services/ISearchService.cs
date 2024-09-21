using System.Threading;
using System.Threading.Tasks;

namespace ACI.HAM.Core.Services
{
    public interface ISearchService<T>
    {
        Task<List<T>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default);

        Task<List<T>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default);
    }
}
