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
    public interface IBoardService : ICRUDService<BoardDto, BoardEditableDto, int>
    {
    }

    public class BoardService : IBoardService
    {
        private readonly BaseContext _baseContext;
        private readonly IMapper _mapper;

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

        public async Task<DataTablesResult<BoardDto>> ReadDataTableAsync(DataTablesParameters dataTablesParameters, string languageCode = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Board> query =
                _baseContext.Boards
                    .Include(x => x.Translations)
                    .Include(x => x.BoardHotelsCompanies)
                    .ThenInclude(x => x.Company)
                    .AsQueryable();
            return await query.GetDataTablesResultAsync<Board, BoardDto>(_mapper, dataTablesParameters, languageCode, cancellationToken);
        }

        public async Task<BoardEditableDto> ReadEditableByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _baseContext.Boards
                .Include(x => x.Translations)
                .Include(x => x.BoardHotelsCompanies)
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
                .Include(x => x.BoardHotelsCompanies)
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
                ICollection<BoardHotelCompany> boardHotelsCompanies = board.BoardHotelsCompanies
                    .ToList();
                foreach (var existingBoardHotelCompany in existingBoard.BoardHotelsCompanies)
                {
                    if (boardHotelsCompanies.All(x => x.CompanyId != existingBoardHotelCompany.CompanyId || x.HotelId != existingBoardHotelCompany.HotelId))
                    {
                        _baseContext.Set<BoardHotelCompany>().Remove(existingBoardHotelCompany);
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
                var boardHotelCompanyPairs = boardHotelsCompanies
                    .GroupJoin(
                        existingBoard.BoardHotelsCompanies,
                        newBoardHotelCompany => new { newBoardHotelCompany.CompanyId, newBoardHotelCompany.HotelId },
                        existingBoardHotelCompany => new { existingBoardHotelCompany.CompanyId, existingBoardHotelCompany.HotelId },
                        (newBoardHotelCompany, existingBoardHotelCompany) => new { newBoardHotelCompany, existingBoardHotelCompany })
                    .SelectMany(
                        x => x.existingBoardHotelCompany.DefaultIfEmpty(),
                        (x, existingBoardHotelCompany) => new { x.newBoardHotelCompany, existingBoardHotelCompany })
                    .ToList();
                foreach (var pair in boardHotelCompanyPairs)
                {
                    pair.newBoardHotelCompany.BoardId = existingBoard.Id;
                    if (pair.existingBoardHotelCompany == null)
                    {
                        _baseContext.Set<BoardHotelCompany>().Add(pair.newBoardHotelCompany);
                    }
                }
                await _baseContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
