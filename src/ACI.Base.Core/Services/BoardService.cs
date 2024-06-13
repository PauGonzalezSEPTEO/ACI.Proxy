using System.Data;
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
    public interface IBoardService : ICRUDService<BoardDto, BoardEditableDto, int>
    {
    }

    public class BoardService : IBoardService
    {
        private readonly IMapper _mapper;
        private readonly BaseContext _baseContext;

        public BoardService(BaseContext baseContext, IMapper mapper)
        {
            _baseContext = baseContext;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(BoardEditableDto boardEditableDto, CancellationToken cancellationToken = default)
        {
            Board board = _mapper.Map<Board>(boardEditableDto);
            _baseContext.Boards.Add(board);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return board.Id;
        }

        public async Task<BoardEditableDto> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Board board = await _baseContext.Boards
                .Include(x => x.Translations)
                .Include(x => x.BoardsBuildings)
                .SingleOrDefaultAsync(x => x.Id == id);
            _baseContext.Remove(board);
            await _baseContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<BoardEditableDto>(board);
        }

        public async Task<DataTablesResult<BoardDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, ClaimsPrincipal claimsPrincipal, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Board> query;
            if (claimsPrincipal.IsInRole("Administrator"))
            {
                query = _baseContext.Boards
                    .Include(x => x.Translations)
                    .AsQueryable();
            }
            else
            {
                List<int> companies = await _baseContext.GetUserCompaniesAsync(claimsPrincipal);
                query = _baseContext.Boards
                    .Include(x => x.Translations)
                    .Where(x => x.BoardsBuildings.Any(y => companies.Contains(y.Building.Hotel.CompanyId)))
                    .AsQueryable();
            }
            return await query.GetDataTablesResultAsync<Board, BoardDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<BoardEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Boards
                .Include(x => x.Translations)
                .Include(x => x.BoardsBuildings)
                .AsNoTracking()
                .ProjectTo<BoardEditableDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateByIdAsync(int id, BoardEditableDto boardEditableDto, CancellationToken cancellationToken = default)
        {
            Board board = _mapper.Map<Board>(boardEditableDto);
            var existingBoard = _baseContext.Set<Board>()
                .Include(x => x.Translations)
                .Include(x => x.BoardsBuildings)
                .SingleOrDefault(x => x.Id == board.Id);
            if (existingBoard != null)
            {
                ICollection<BoardTranslation> translations = board.Translations
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.ShortDescription))
                    .ToList();
                foreach (var existingTranslation in existingBoard.Translations.ToList())
                {
                    if (translations.All(x => x.LanguageCode != existingTranslation.LanguageCode))
                    {
                        _baseContext.Set<BoardTranslation>().Remove(existingTranslation);
                    }
                }
                ICollection<BoardBuilding> buildings = board.BoardsBuildings
                    .ToList();
                foreach (var existingBuilding in existingBoard.BoardsBuildings)
                {
                    if (buildings.All(x => x.BuildingId != existingBuilding.BuildingId))
                    {
                        _baseContext.Set<BoardBuilding>().Remove(existingBuilding);
                    }
                }
                _baseContext.Entry(existingBoard).CurrentValues.SetValues(board);
                var translationPairs = translations
                    .GroupJoin(
                        existingBoard.Translations,
                        newTranslation => newTranslation.LanguageCode,
                        existingTranslation => existingTranslation.LanguageCode,
                        (newTranslation, existingTranslation) => new { newTranslation, existingTranslation })
                    .SelectMany(
                        x => x.existingTranslation.DefaultIfEmpty(),
                        (x, existingTranslation) => new { x.newTranslation, existingTranslation })
                    .ToList();
                foreach (var pair in translationPairs)
                {
                    pair.newTranslation.BoardId = existingBoard.Id;
                    if (pair.existingTranslation != null)
                    {
                        _baseContext.Entry(pair.existingTranslation).CurrentValues.SetValues(pair.newTranslation);
                    }
                    else
                    {
                        _baseContext.Set<BoardTranslation>().Add(pair.newTranslation);
                    }
                }
                var buildingPairs = buildings
                    .GroupJoin(
                        existingBoard.BoardsBuildings,
                        newBuilding => newBuilding.BuildingId,
                        existingBuilding => existingBuilding.BuildingId,
                        (newBuilding, existingBuilding) => new { newBuilding, existingBuilding })
                    .SelectMany(
                        x => x.existingBuilding.DefaultIfEmpty(),
                        (x, existingBuilding) => new { x.newBuilding, existingBuilding })
                    .ToList();
                foreach (var pair in buildingPairs)
                {
                    pair.newBuilding.BoardId = existingBoard.Id;
                    if (pair.existingBuilding != null)
                    {
                        _baseContext.Entry(pair.existingBuilding).CurrentValues.SetValues(pair.newBuilding);
                    }
                    else
                    {
                        _baseContext.Set<BoardBuilding>().Add(pair.newBuilding);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
