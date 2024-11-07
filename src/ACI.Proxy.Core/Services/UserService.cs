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
                .Include(x => x.UserProjectsCompanies)
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
                .Include(x => x.UserProjectsCompanies)
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
                .Include(x => x.UserProjectsCompanies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetDataTablesResultAsync<User, UserDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<UserEditableDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserProjectsCompanies)
                .AsNoTracking()
                .ProjectTo<UserEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<UserDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserProjectsCompanies)
                .ThenInclude(x => x.Company)
                .AsQueryable();
            return await query.GetResultAsync<User, UserDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(string id, UserEditableDto userEditableDto, CancellationToken cancellationToken = default)
        {
            UpdatableUser user = _mapper.Map<UpdatableUser>(userEditableDto);
            var existingUser = _baseContext.Set<User>()
                .Include(x => x.UserRoles)
                .Include(x => x.UserProjectsCompanies)
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
                ICollection<UserProjectCompany> userProjectsCompanies = user.UserProjectsCompanies
                    .ToList();
                foreach (var existingUserProjectCompany in existingUser.UserProjectsCompanies)
                {
                    if (userProjectsCompanies.All(x => x.CompanyId != existingUserProjectCompany.CompanyId || x.ProjectId != existingUserProjectCompany.ProjectId))
                    {
                        _baseContext.Set<UserProjectCompany>().Remove(existingUserProjectCompany);
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
                var userProjectCompanyPairs = userProjectsCompanies
                    .GroupJoin(
                        existingUser.UserProjectsCompanies,
                        newUserProjectCompany => new { newUserProjectCompany.CompanyId, newUserProjectCompany.ProjectId },
                        existingUserProjectCompany => new { existingUserProjectCompany.CompanyId, existingUserProjectCompany.ProjectId },
                        (newUserProjectCompany, existingUserProjectCompany) => new { newUserProjectCompany, existingUserProjectCompany })
                    .SelectMany(
                        x => x.existingUserProjectCompany.DefaultIfEmpty(),
                        (x, existingUserProjectCompany) => new { x.newUserProjectCompany, existingUserProjectCompany })
                    .ToList();
                foreach (var pair in userProjectCompanyPairs)
                {
                    pair.newUserProjectCompany.UserId = existingUser.Id;
                    if (pair.existingUserProjectCompany == null)
                    {
                        _baseContext.Set<UserProjectCompany>().Add(pair.newUserProjectCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
