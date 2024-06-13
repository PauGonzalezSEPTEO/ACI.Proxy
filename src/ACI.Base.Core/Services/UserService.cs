using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACI.Base.Core.Data;
using ACI.Base.Core.Dtos;
using ACI.Base.Core.Extensions;
using ACI.Base.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ACI.Base.Core.Services
{
    public interface IUserService : ICRUDService<UserDto, UserEditableDto, string>, ISearchService<UserDto>
    {
    }

    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly BaseContext _baseContext;

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
                .Include(x => x.Companies)
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
                .Include(x => x.Companies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetResultAsync<User, UserDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<UserDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, ClaimsPrincipal claimsPrincipal, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.Translations)
                .Include(x => x.Companies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<User, UserDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<UserEditableDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.Companies)
                .ThenInclude(x => x.Company)
                .AsNoTracking()
                .ProjectTo<UserEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<UserDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.Companies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetResultAsync<User, UserDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(string id, UserEditableDto userEditableDto, CancellationToken cancellationToken = default)
        {
            UpdatableUser user = _mapper.Map<UpdatableUser>(userEditableDto);
            var existingUser = _baseContext.Set<User>()
                .Include(x => x.UserRoles)
                .Include(x => x.Companies)
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
                ICollection<UserCompany> companies = user.UserCompanies
                    .ToList();
                foreach (var existingCompany in existingUser.Companies)
                {
                    if (companies.All(x => x.CompanyId != existingCompany.CompanyId))
                    {
                        _baseContext.Set<UserCompany>().Remove(existingCompany);
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
                    if (pair.existingRole != null)
                    {
                        _baseContext.Entry(pair.existingRole).CurrentValues.SetValues(pair.newRole);
                    }
                    else
                    {
                        _baseContext.Set<UserRole>().Add(pair.newRole);
                    }
                }
                var companyPairs = companies
                    .GroupJoin(
                        existingUser.Companies,
                        newCompany => newCompany.CompanyId,
                        existingCompany => existingCompany.CompanyId,
                        (newCompany, existingCompany) => new { newCompany, existingCompany })
                    .SelectMany(
                        x => x.existingCompany.DefaultIfEmpty(),
                        (x, existingCompany) => new { x.newCompany, existingCompany })
                    .ToList();
                foreach (var pair in companyPairs)
                {
                    pair.newCompany.UserId = existingUser.Id;
                    if (pair.existingCompany != null)
                    {
                        _baseContext.Entry(pair.existingCompany).CurrentValues.SetValues(pair.newCompany);
                    }
                    else
                    {
                        _baseContext.Set<UserCompany>().Add(pair.newCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
