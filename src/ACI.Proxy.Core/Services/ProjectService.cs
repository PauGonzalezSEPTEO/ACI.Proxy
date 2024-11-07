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
    public interface IProjectService : ICRUDService<ProjectDto, ProjectEditableDto, int>, ISearchService<ProjectDto>
    {
        Task<List<ProjectDto>> ReadByCompanyIdsAsync(int[] companyIds, CancellationToken cancellationToken = default);
    }

    public class ProjectService : IProjectService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public ProjectService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(ProjectEditableDto projectEditableDto, CancellationToken cancellationToken = default)
        {
            Project project = _mapper.Map<Project>(projectEditableDto);
            _baseContext.Projects.Add(project);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return project.Id;
        }

        public async Task<ProjectEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Project project = await _baseContext.Projects
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(project);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProjectEditableDto>(project);
        }

        public async Task<List<ProjectDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Projects.AsQueryable();
            return await query.GetResultAsync<Project, ProjectDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<List<ProjectDto>> ReadByCompanyIdsAsync(int[] companyIds, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Projects
                .Where(x => companyIds.Contains(x.CompanyId))
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<DataTablesResult<ProjectDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Project> query =
                _baseContext.Projects
                    .AsQueryable();
            return await query.GetDataTablesResultAsync<Project, ProjectDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<ProjectEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Projects
                .AsNoTracking()
                .ProjectTo<ProjectEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ProjectDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Projects.AsQueryable();
            return await query.GetResultAsync<Project, ProjectDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, ProjectEditableDto projectEditableDto, CancellationToken cancellationToken = default)
        {
            Project project = _mapper.Map<Project>(projectEditableDto);
            var existingProject = _baseContext.Set<Project>()
                .SingleOrDefault(x => x.Id == project.Id);
            if (existingProject != null)
            {
                _baseContext.Entry(existingProject).CurrentValues.SetValues(project);
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
