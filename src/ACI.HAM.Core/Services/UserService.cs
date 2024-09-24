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
    public interface IUserService : ICRUDService<UserDto, UserEditableDto, string>, ISearchService<UserDto>
    {
    }

    public class UserService : IUserService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public UserService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public Task<string> CreateAsync(UserEditableDto editableDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserEditableDto> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            User user = await _baseContext.Users
                .Include(x => x.UserRoles)
                .Include(x => x.UserHotelsCompanies)
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(user);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserEditableDto>(user);
        }

        public async Task<List<UserDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserHotelsCompanies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetResultAsync<User, UserDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<UserDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.Translations)
                .Include(x => x.UserHotelsCompanies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<User, UserDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<UserEditableDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserHotelsCompanies)
                .AsNoTracking()
                .ProjectTo<UserEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<UserDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserHotelsCompanies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetResultAsync<User, UserDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(string id, UserEditableDto userEditableDto, CancellationToken cancellationToken = default)
        {
            UpdatableUser user = _mapper.Map<UpdatableUser>(userEditableDto);
            var existingUser = _baseContext.Set<User>()
                .Include(x => x.UserRoles)
                .Include(x => x.UserHotelsCompanies)
                .SingleOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                ICollection<UserRole> roles = user.UserRoles                    
                    .ToList();
                foreach (var existingRole in existingUser.UserRoles)
                {
                    if (roles.All(x => x.RoleId != existingRole.RoleId))
                    {
                        _baseContext.Set<UserRole>().Remove(existingRole);
                    }
                }
                ICollection<UserHotelCompany> userHotelsCompanies = user.UserHotelsCompanies
                    .ToList();
                foreach (var existingUserHotelCompany in existingUser.UserHotelsCompanies)
                {
                    if (userHotelsCompanies.All(x => x.CompanyId != existingUserHotelCompany.CompanyId || x.HotelId != existingUserHotelCompany.HotelId))
                    {
                        _baseContext.Set<UserHotelCompany>().Remove(existingUserHotelCompany);
                    }
                }
                _baseContext.Entry(existingUser).CurrentValues.SetValues(user);
                var rolePairs = roles
                    .GroupJoin(
                        existingUser.UserRoles,
                        newRole => newRole.RoleId,
                        existingRole => existingRole.RoleId,
                        (newRole, existingRole) => new { newRole, existingRole })
                    .SelectMany(
                        x => x.existingRole.DefaultIfEmpty(),
                        (x, existingRole) => new { x.newRole, existingRole })
                    .ToList();
                foreach (var pair in rolePairs)
                {
                    pair.newRole.UserId = existingUser.Id;
                    if (pair.existingRole == null)
                    {
                        _baseContext.Set<UserRole>().Add(pair.newRole);
                    }
                }
                var userHotelCompanyPairs = userHotelsCompanies
                    .GroupJoin(
                        existingUser.UserHotelsCompanies,
                        newUserHotelCompany => new { newUserHotelCompany.CompanyId, newUserHotelCompany.HotelId },
                        existingUserHotelCompany => new { existingUserHotelCompany.CompanyId, existingUserHotelCompany.HotelId },
                        (newUserHotelCompany, existingUserHotelCompany) => new { newUserHotelCompany, existingUserHotelCompany })
                    .SelectMany(
                        x => x.existingUserHotelCompany.DefaultIfEmpty(),
                        (x, existingUserHotelCompany) => new { x.newUserHotelCompany, existingUserHotelCompany })
                    .ToList();
                foreach (var pair in userHotelCompanyPairs)
                {
                    pair.newUserHotelCompany.UserId = existingUser.Id;
                    if (pair.existingUserHotelCompany == null)
                    {
                        _baseContext.Set<UserHotelCompany>().Add(pair.newUserHotelCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
