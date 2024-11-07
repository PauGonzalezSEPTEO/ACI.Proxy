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
    public interface ITemplateService : ICRUDService<TemplateDto, TemplateEditableDto, int>
    {
    }

    public class TemplateService : ITemplateService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

        public TemplateService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(TemplateEditableDto templateEditableDto, CancellationToken cancellationToken = default)
        {
            Template template = _mapper.Map<Template>(templateEditableDto);
            _baseContext.Templates.Add(template);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return template.Id;
        }

        public async Task<TemplateEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Template template = await _baseContext.Templates
                .Include(x => x.Translations)
                .Include(x => x.TemplatesBuildings)
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(template);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TemplateEditableDto>(template);
        }

        public async Task<DataTablesResult<TemplateDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Template> query =
                _baseContext.Templates
                    .Include(x => x.Translations)
                    .Include(x => x.TemplateProjectsCompanies)
                    .ThenInclude(x => x.Company)
                    .AsQueryable();
            return await query.GetDataTablesResultAsync<Template, TemplateDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<TemplateEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Templates
                .Include(x => x.Translations)
                .Include(x => x.TemplateProjectsCompanies)
                .Include(x => x.TemplatesBuildings)
                .AsNoTracking()
                .ProjectTo<TemplateEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, TemplateEditableDto templateEditableDto, CancellationToken cancellationToken = default)
        {
            Template template = _mapper.Map<Template>(templateEditableDto);
            var existingTemplate = _baseContext.Set<Template>()
                .Include(x => x.Translations)
                .Include(x => x.TemplatesBuildings)
                .Include(x => x.TemplateProjectsCompanies)
                .SingleOrDefault(x => x.Id == template.Id);
            if (existingTemplate != null)
            {
                ICollection<TemplateTranslation> translations = template.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingTemplate.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<TemplateTranslation>().Remove(existingTranslation);
                    }
                }
                ICollection<TemplateBuilding> buildings = template.TemplatesBuildings
                    .ToList();
                foreach (var existingBuilding in existingTemplate.TemplatesBuildings)
                {
                    if (buildings.All(x => x.BuildingId != existingBuilding.BuildingId))
                    {
                        _baseContext.Set<TemplateBuilding>().Remove(existingBuilding);
                    }
                }
                ICollection<TemplateProjectCompany> templateProjectsCompanies = template.TemplateProjectsCompanies
                    .ToList();
                foreach (var existingTemplateProjectCompany in existingTemplate.TemplateProjectsCompanies)
                {
                    if (templateProjectsCompanies.All(x => x.CompanyId != existingTemplateProjectCompany.CompanyId || x.ProjectId != existingTemplateProjectCompany.ProjectId))
                    {
                        _baseContext.Set<TemplateProjectCompany>().Remove(existingTemplateProjectCompany);
                    }
                }
                _baseContext.Entry(existingTemplate).CurrentValues.SetValues(template);
                var translationPairs = translations
                    .GroupJoin(
                        existingTemplate.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.TemplateId = existingTemplate.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<TemplateTranslation>().Add(pair.newTranslation);
                    }
                }
                var buildingPairs = buildings
                    .GroupJoin(
                        existingTemplate.TemplatesBuildings,
                        newBuilding => newBuilding.BuildingId,
                        existingBuilding => existingBuilding.BuildingId,
                        (newBuilding, existingBuilding) => new { newBuilding, existingBuilding })
                    .SelectMany(
                        x => x.existingBuilding.DefaultIfEmpty(),
                        (x, existingBuilding) => new { x.newBuilding, existingBuilding })
                    .ToList();
                foreach (var pair in buildingPairs)
                {
                    pair.newBuilding.TemplateId = existingTemplate.Id;
                    if (pair.existingBuilding != null)
                    {
                        _baseContext.Entry(pair.existingBuilding).CurrentValues.SetValues(pair.newBuilding);
                    }
                    else
                    {
                        _baseContext.Set<TemplateBuilding>().Add(pair.newBuilding);
                    }
                }
                var templateProjectCompanyPairs = templateProjectsCompanies
                    .GroupJoin(
                        existingTemplate.TemplateProjectsCompanies,
                        newTemplateProjectCompany => new { newTemplateProjectCompany.CompanyId, newTemplateProjectCompany.ProjectId },
                        existingTemplateProjectCompany => new { existingTemplateProjectCompany.CompanyId, existingTemplateProjectCompany.ProjectId },
                        (newTemplateProjectCompany, existingTemplateProjectCompany) => new { newTemplateProjectCompany, existingTemplateProjectCompany })
                    .SelectMany(
                        x => x.existingTemplateProjectCompany.DefaultIfEmpty(),
                        (x, existingTemplateProjectCompany) => new { x.newTemplateProjectCompany, existingTemplateProjectCompany })
                    .ToList();
                foreach (var pair in templateProjectCompanyPairs)
                {
                    pair.newTemplateProjectCompany.TemplateId = existingTemplate.Id;
                    if (pair.existingTemplateProjectCompany == null)
                    {
                        _baseContext.Set<TemplateProjectCompany>().Add(pair.newTemplateProjectCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
