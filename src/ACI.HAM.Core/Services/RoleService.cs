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
    public interface IRoleService : ICRUDService<RoleDto, RoleEditableDto, string>, ISearchService<RoleDto>
    {
        Task<bool> DeleteUserByIdAsync(string id, string userId, CancellationToken cancellationToken = default);

        Task<DataTablesResult<UserDto>> ReadUsersDataTableAsync(string id, DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default);
    }

    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly BaseContext _baseContext;

        public RoleService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<string> CreateAsync(RoleEditableDto roleEditableDto, CancellationToken cancellationToken = default)
        {            
            Role role = _mapper.Map<Role>(roleEditableDto);
            role.Id = Guid.NewGuid().ToString();
            _baseContext.Roles.Add(role);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return role.Id;
        }

        public async Task<RoleEditableDto> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            Role role = await _baseContext.Roles
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(role);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<RoleEditableDto>(role);
        }

        public async Task<bool> DeleteUserByIdAsync(string id, string userId, CancellationToken cancellationToken = default)
        {
            UserRole userRole = await _baseContext.UserRoles
                .SingleOrDefaultAsync(x => (x.UserId == userId) && (x.RoleId == id));
            _baseContext.Remove(userRole);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<RoleDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Roles
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Role, RoleDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<RoleDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, ClaimsPrincipal claimsPrincipal, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Roles.AsQueryable();
            return await query.GetDataTablesResultAsync<Role, RoleDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<UserDto>> ReadUsersDataTableAsync(string id, DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.Translations)
                .Where(x => x.UserRoles.Any(y => y.RoleId == id))
                .AsQueryable();
            return await query.GetDataTablesResultAsync<User, UserDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<RoleEditableDto> ReadEditableByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Roles
                .Include(x => x.Translations)
                .AsNoTracking()
                .ProjectTo<RoleEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<RoleDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Roles
                .Include(x => x.Translations)
                .AsQueryable();
            return await query.GetResultAsync<Role, RoleDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(string id, RoleEditableDto roleEditableDto, CancellationToken cancellationToken = default)
        {
            Role role = _mapper.Map<Role>(roleEditableDto);
            var existingRole = _baseContext.Set<Role>()
                .Include(x => x.Translations)
                .SingleOrDefault(x => x.Id == role.Id);
            if (existingRole != null)
            {
                ICollection<RoleTranslation> translations = role.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingRole.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<RoleTranslation>().Remove(existingTranslation);
                    }
                }
                _baseContext.Entry(existingRole).CurrentValues.SetValues(role);
                var translationPairs = translations
                    .GroupJoin(
                        existingRole.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.RoleId = existingRole.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<RoleTranslation>().Add(pair.newTranslation);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
