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
using System.Security.Claims;

namespace ACI.HAM.Core.Services
{
    public interface IHotelService : ICRUDService<HotelDto, HotelEditableDto, int>, ISearchService<HotelDto>
    {
        Task<List<HotelDto>> ReadByCompanyIdsAsync(int[] companyIds, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);
    }

    public class HotelService : IHotelService
    {
        private readonly IMapper _mapper;
        private readonly BaseContext _baseContext;

        public HotelService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(HotelEditableDto hotelEditableDto, CancellationToken cancellationToken = default)
        {
            Hotel hotel = _mapper.Map<Hotel>(hotelEditableDto);
            _baseContext.Hotels.Add(hotel);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return hotel.Id;
        }

        public async Task<HotelEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Hotel hotel = await _baseContext.Hotels
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(hotel);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<HotelEditableDto>(hotel);
        }

        public async Task<List<HotelDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Hotels.AsQueryable();
            return await query.GetResultAsync<Hotel, HotelDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<List<HotelDto>> ReadByCompanyIdsAsync(int[] companyIds, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
        {
            List<int> commonCompanies;
            if (claimsPrincipal.IsInRole("Administrator"))
            {
                commonCompanies = companyIds.ToList();
            }
            else
            {
                List<int> companies = await _baseContext.GetUserCompaniesAsync(claimsPrincipal);
                commonCompanies = companyIds.Intersect(companies).ToList();
            }
            return await _baseContext.Hotels
                .Where(x => commonCompanies.Contains(x.CompanyId))
                .ProjectTo<HotelDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<DataTablesResult<HotelDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, ClaimsPrincipal claimsPrincipal, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Hotel> query;
            if (claimsPrincipal.IsInRole("Administrator"))
            {
                query = _baseContext.Hotels
                    .AsQueryable();
            }
            else
            {
                List<int> companies = await _baseContext.GetUserCompaniesAsync(claimsPrincipal);
                query = _baseContext.Hotels
                    .Where(x => companies.Contains(x.CompanyId))
                    .AsQueryable();
            }
            return await query.GetDataTablesResultAsync<Hotel, HotelDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<HotelEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Hotels
                .AsNoTracking()
                .ProjectTo<HotelEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<HotelDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Hotels.AsQueryable();
            return await query.GetResultAsync<Hotel, HotelDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, HotelEditableDto hotelEditableDto, CancellationToken cancellationToken = default)
        {
            Hotel hotel = _mapper.Map<Hotel>(hotelEditableDto);
            var existingHotel = _baseContext.Set<Hotel>()
                .SingleOrDefault(x => x.Id == hotel.Id);
            if (existingHotel != null)
            {
                _baseContext.Entry(existingHotel).CurrentValues.SetValues(hotel);
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
