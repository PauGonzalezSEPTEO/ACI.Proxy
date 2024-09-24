using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.HAM.Core.Data;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Extensions;
using ACI.HAM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Services
{
    public interface IBuildingService : ICRUDService<BuildingDto, BuildingEditableDto, int>, ISearchService<BuildingDto>
    {
    }

    public class BuildingService : IBuildingService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public BuildingService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(BuildingEditableDto buildingEditableDto, CancellationToken cancellationToken = default)
        {
            Building building = _mapper.Map<Building>(buildingEditableDto);
            _baseContext.Buildings.Add(building);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return building.Id;
        }

        public async Task<BuildingEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Building building = await _baseContext.Buildings
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(building);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<BuildingEditableDto>(building);
        }

        public async Task<List<BuildingDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Buildings
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Building, BuildingDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<BuildingDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Building> query = _baseContext.Buildings
                .Include(x => x.Translations)
                .Include(x => x.Hotel)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<Building, BuildingDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<BuildingEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Buildings
                .Include(x => x.Translations)
                .AsNoTracking()
                .ProjectTo<BuildingEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<BuildingDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Buildings
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Building, BuildingDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, BuildingEditableDto buildingEditableDto, CancellationToken cancellationToken = default)
        {
            Building building = _mapper.Map<Building>(buildingEditableDto);
            var existingBuilding = _baseContext.Set<Building>()
                .Include(x => x.Translations)
                .SingleOrDefault(x => x.Id == building.Id);
            if (existingBuilding != null)
            {
                ICollection<BuildingTranslation> translations = building.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingBuilding.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<BuildingTranslation>().Remove(existingTranslation);
                    }
                }
                _baseContext.Entry(existingBuilding).CurrentValues.SetValues(building);
                var translationPairs = translations
                    .GroupJoin(
                        existingBuilding.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.BuildingId = existingBuilding.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<BuildingTranslation>().Add(pair.newTranslation);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
