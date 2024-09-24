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
    public interface IRoomTypeService : ICRUDService<RoomTypeDto, RoomTypeEditableDto, int>
    {
    }

    public class RoomTypeService : IRoomTypeService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public RoomTypeService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(RoomTypeEditableDto roomTypeEditableDto, CancellationToken cancellationToken = default)
        {
            RoomType roomType = _mapper.Map<RoomType>(roomTypeEditableDto);
            _baseContext.RoomTypes.Add(roomType);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return roomType.Id;
        }

        public async Task<RoomTypeEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            RoomType roomType = await _baseContext.RoomTypes
                .Include(x => x.Translations)
                .Include(x => x.RoomTypesBuildings)
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(roomType);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<RoomTypeEditableDto>(roomType);
        }

        public async Task<DataTablesResult<RoomTypeDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<RoomType> query = _baseContext.RoomTypes
                .Include(x => x.Translations)
                .Include(x => x.RoomTypeHotelsCompanies)
                .ThenInclude(x => x.Company)
                .Include(x => x.RoomTypesBuildings)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<RoomType, RoomTypeDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<RoomTypeEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.RoomTypes
                .Include(x => x.Translations)
                .Include(x => x.RoomTypeHotelsCompanies)
                .Include(x => x.RoomTypesBuildings)
                .AsNoTracking()
                .ProjectTo<RoomTypeEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, RoomTypeEditableDto roomTypeEditableDto, CancellationToken cancellationToken = default)
        {
            RoomType roomType = _mapper.Map<RoomType>(roomTypeEditableDto);
            var existingRoomType = _baseContext.Set<RoomType>()
                .Include(x => x.Translations)
                .Include(x => x.RoomTypesBuildings)
                .Include(x => x.RoomTypeHotelsCompanies)
                .SingleOrDefault(x => x.Id == roomType.Id);
            if (existingRoomType != null)
            {
                ICollection<RoomTypeTranslation> translations = roomType.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingRoomType.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<RoomTypeTranslation>().Remove(existingTranslation);
                    }
                }
                ICollection<RoomTypeBuilding> buildings = roomType.RoomTypesBuildings
                    .ToList();
                foreach (var existingBuilding in existingRoomType.RoomTypesBuildings)
                {
                    if (buildings.All(x => x.BuildingId != existingBuilding.BuildingId))
                    {
                        _baseContext.Set<RoomTypeBuilding>().Remove(existingBuilding);
                    }
                }
                ICollection<RoomTypeHotelCompany> roomTypeHotelsCompanies = roomType.RoomTypeHotelsCompanies
                    .ToList();
                foreach (var existingRoomTypeHotelCompany in existingRoomType.RoomTypeHotelsCompanies)
                {
                    if (roomTypeHotelsCompanies.All(x => x.CompanyId != existingRoomTypeHotelCompany.CompanyId || x.HotelId != existingRoomTypeHotelCompany.HotelId))
                    {
                        _baseContext.Set<RoomTypeHotelCompany>().Remove(existingRoomTypeHotelCompany);
                    }
                }
                _baseContext.Entry(existingRoomType).CurrentValues.SetValues(roomType);
                var translationPairs = translations
                    .GroupJoin(
                        existingRoomType.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.RoomTypeId = existingRoomType.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<RoomTypeTranslation>().Add(pair.newTranslation);
                    }
                }
                var buildingPairs = buildings
                    .GroupJoin(
                        existingRoomType.RoomTypesBuildings,
                        newBuilding => newBuilding.BuildingId,
                        existingBuilding => existingBuilding.BuildingId,
                        (newBuilding, existingBuilding) => new { newBuilding, existingBuilding })
                    .SelectMany(
                        x => x.existingBuilding.DefaultIfEmpty(),
                        (x, existingBuilding) => new { x.newBuilding, existingBuilding })
                    .ToList();
                foreach (var pair in buildingPairs)
                {
                    pair.newBuilding.RoomTypeId = existingRoomType.Id;
                    if (pair.existingBuilding != null)
                    {
                        _baseContext.Entry(pair.existingBuilding).CurrentValues.SetValues(pair.newBuilding);
                    }
                    else
                    {
                        _baseContext.Set<RoomTypeBuilding>().Add(pair.newBuilding);
                    }
                }
                var roomTypeHotelCompanyPairs = roomTypeHotelsCompanies
                    .GroupJoin(
                        existingRoomType.RoomTypeHotelsCompanies,
                        newRoomTypeHotelCompany => new { newRoomTypeHotelCompany.CompanyId, newRoomTypeHotelCompany.HotelId },
                        existingRoomTypeHotelCompany => new { existingRoomTypeHotelCompany.CompanyId, existingRoomTypeHotelCompany.HotelId },
                        (newRoomTypeHotelCompany, existingRoomTypeHotelCompany) => new { newRoomTypeHotelCompany, existingRoomTypeHotelCompany })
                    .SelectMany(
                        x => x.existingRoomTypeHotelCompany.DefaultIfEmpty(),
                        (x, existingRoomTypeHotelCompany) => new { x.newRoomTypeHotelCompany, existingRoomTypeHotelCompany })
                    .ToList();
                foreach (var pair in roomTypeHotelCompanyPairs)
                {
                    pair.newRoomTypeHotelCompany.RoomTypeId = existingRoomType.Id;
                    if (pair.existingRoomTypeHotelCompany == null)
                    {
                        _baseContext.Set<RoomTypeHotelCompany>().Add(pair.newRoomTypeHotelCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
