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
    public interface ICompanyService : ICRUDService<CompanyDto, CompanyEditableDto, int>, ISearchService<CompanyDto>
    {
    }

    public class CompanyService : ICompanyService
    {
        private readonly IMapper _mapper;
        private readonly BaseContext _baseContext;

        public CompanyService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CompanyEditableDto companyEditableDto, CancellationToken cancellationToken = default)
        {
            Company company = _mapper.Map<Company>(companyEditableDto);
            _baseContext.Companies.Add(company);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return company.Id;
        }

        public async Task<CompanyEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Company company = await _baseContext.Companies
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(company);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CompanyEditableDto>(company);
        }

        public async Task<List<CompanyDto>> ReadAsync(string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Companies.AsQueryable();
            return await query.GetResultAsync<Company, CompanyDto>(_mapper, null, null, languageCode, cancellationToken);
        }

        public async Task<DataTablesResult<CompanyDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, ClaimsPrincipal claimsPrincipal, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Company> query;
            if (claimsPrincipal.IsInRole("Administrator"))
            {
                query = _baseContext.Companies
                    .AsQueryable();
            }
            else
            {
                List<int> companies = await _baseContext.GetUserCompaniesAsync(claimsPrincipal);
                query = _baseContext.Companies
                    .Where(x => companies.Contains(x.Id))
                    .AsQueryable();
            }
            return await query.GetDataTablesResultAsync<Company, CompanyDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<CompanyEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Companies
                .AsNoTracking()
                .ProjectTo<CompanyEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<CompanyDto>> SearchAsync(string search = null, string ordering = null, string languageCode = null, CancellationToken cancellationToken = default)
        {
            var query = _baseContext.Companies.AsQueryable();
            return await query.GetResultAsync<Company, CompanyDto>(_mapper, search, ordering, languageCode, cancellationToken);
        }


        public async Task<bool> UpdateByIdAsync(int id, CompanyEditableDto companyEditableDto, CancellationToken cancellationToken = default)
        {
            Company company = _mapper.Map<Company>(companyEditableDto);
            var existingCompany = _baseContext.Set<Company>()
                .SingleOrDefault(x => x.Id == company.Id);
            if (existingCompany != null)
            {
                _baseContext.Entry(existingCompany).CurrentValues.SetValues(company);
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
