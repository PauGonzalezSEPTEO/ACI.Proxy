using ACI.HAM.Core.Dtos;
using System.Threading.Tasks;
using System.Threading;

namespace ACI.HAM.Core.Services
{
    public interface ICRUDService<T, U, V>
    {
        Task<V> CreateAsync(U editableDto, CancellationToken cancellationToken);

        Task<U> DeleteByIdAsync(V id, CancellationToken cancellationToken = default);

        Task<DataTablesResult<T>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default);

        Task<U> ReadEditableByIdAsync(V id, CancellationToken cancellationToken = default);

        Task<bool> UpdateByIdAsync(V id, U editableDto, CancellationToken cancellationToken = default);
    }
}
