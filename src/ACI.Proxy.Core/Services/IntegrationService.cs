using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.Proxy.Core.Data;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Extensions;
using ACI.Proxy.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ACI.Proxy.Core.Services
{
    public interface IIntegrationService : ICRUDService<IntegrationDto, IntegrationEditableDto, int>, ISearchService<IntegrationDto>
    {
        Task<List<IntegrationDto>> ReadByProjectIdsAsync(int[] projectIds, string languageCode = null, CancellationToken cancellationToken = default);
    }

    public class IntegrationService : IIntegrationService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public IntegrationService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(IntegrationEditableDto integrationEditableDto, CancellationToken cancellationToken = default)
        {
            Integration integration = _mapper.Map<Integration>(integrationEditableDto);
            _baseContext.Integrations.Add(integration);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return integration.Id;
        }

        public async Task<IntegrationEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Integration integration = await _baseContext.Integrations
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(integration);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<IntegrationEditableDto>(integration);
        }

        public async Task<List<IntegrationDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Integrations
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Integration, IntegrationDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<List<IntegrationDto>> ReadByProjectIdsAsync(int[] projectIds, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Integrations
                .Where(x => projectIds.Contains(x.ProjectId))
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Integration, IntegrationDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<IntegrationDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Integration> query = _baseContext.Integrations
                .Include(x => x.Translations)
                .Include(x => x.Project)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<Integration, IntegrationDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<IntegrationEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Integrations
                .Include(x => x.Translations)
                .AsNoTracking()
                .ProjectTo<IntegrationEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<IntegrationDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Integrations
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Integration, IntegrationDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, IntegrationEditableDto integrationEditableDto, CancellationToken cancellationToken = default)
        {
            Integration integration = _mapper.Map<Integration>(integrationEditableDto);
            var existingIntegration = _baseContext.Set<Integration>()
                .Include(x => x.Translations)
                .SingleOrDefault(x => x.Id == integration.Id);
            if (existingIntegration != null)
            {
                ICollection<IntegrationTranslation> translations = integration.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingIntegration.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<IntegrationTranslation>().Remove(existingTranslation);
                    }
                }
                _baseContext.Entry(existingIntegration).CurrentValues.SetValues(integration);
                var translationPairs = translations
                    .GroupJoin(
                        existingIntegration.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.IntegrationId = existingIntegration.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<IntegrationTranslation>().Add(pair.newTranslation);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
