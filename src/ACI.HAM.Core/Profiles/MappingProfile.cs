using System.Linq;
using AutoMapper;
using ACI.HAM.Core.Dtos;
using ACI.HAM.Core.Models;

namespace ACI.HAM.Core.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            string languageCode = null;
            CreateMap<RoleTranslationDto, RoleTranslation>()
                .ReverseMap();
            CreateMap<RoleEditableDto, Role>()
                .ForMember(x => x.NormalizedName, opt => opt.MapFrom(x => x.Name.ToUpperInvariant()))
                .ReverseMap();
            CreateMap<Role, RoleDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations))
                .ForMember(x => x.TotalUsers, opt => opt.MapFrom(x => x.UserRoles.Count));
            CreateMap<CreateUserDto, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(x => x.UserRoles, opt => opt.MapFrom(x => x.Roles.Select(y => new UserRole() { RoleId = y }).ToList()))
                .ForMember(x => x.Companies, opt => opt.MapFrom(x => x.Companies.Select(y => new UserCompany() { CompanyId = y }).ToList()));    
            CreateMap<RegistrationDto, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email));
            CreateMap<User, AccountDto>().ReverseMap();
            CreateMap<ProfileDetailsDto, User>();
            CreateMap<User, UserEditableDto>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(x => x.UserRoles.Select(y => y.RoleId).ToList()))
                .ForMember(x => x.Companies, opt => opt.MapFrom(x => x.Companies.Select(y => y.CompanyId).ToList()))
                .ReverseMap();
            CreateMap<UserEditableDto, UpdatableUser>()
                .ForMember(x => x.UserRoles, opt => opt.MapFrom(x => x.Roles.Select(y => new UserRole() { UserId = x.Id, RoleId = y }).ToList()))
                .ForMember(x => x.UserCompanies, opt => opt.MapFrom(x => x.Companies.Select(y => new UserCompany() { UserId = x.Id, CompanyId = y }).ToList()));
            CreateMap<User, UserDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Roles, opt => opt.MapFrom(x => x.UserRoles.Select(y => y.Role)))
                .ForMember(x => x.Companies, opt => opt.MapFrom(x => x.Companies.Select(y => y.Company)));
            CreateMap<CompanyEditableDto, Company>().ReverseMap();
            CreateMap<Company, CompanyDto>();
            CreateMap<HotelEditableDto, Hotel>().ReverseMap();
            CreateMap<Hotel, HotelDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name));
            CreateMap<BoardTranslationDto, BoardTranslation>()
                .ReverseMap();
            CreateMap<Board, BoardEditableDto>()
                .ForMember(x => x.Buildings, opt => opt.MapFrom(x => x.BoardsBuildings.Select(y => y.BuildingId).ToList()))
                .ReverseMap()
                .ForMember(x => x.BoardsBuildings, opt => opt.MapFrom(x => x.Buildings.Select(y => new BoardBuilding() { BuildingId = y, BoardId = x.Id }).ToList()));
            CreateMap<Board, BoardDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
            CreateMap<BuildingTranslationDto, BuildingTranslation>()
                .ReverseMap();
            CreateMap<BuildingEditableDto, Building>()
                .ReverseMap();
            CreateMap<Building, BuildingDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations))
                .ForMember(x => x.HotelName, opt => opt.MapFrom(x => x.Hotel.Name));
            CreateMap<Building, BuildingEditableDto>()
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
            CreateMap<RoomTypeTranslationDto, RoomTypeTranslation>()
                .ReverseMap();
            CreateMap<RoomType, RoomTypeEditableDto>()
                .ForMember(x => x.Buildings, opt => opt.MapFrom(x => x.RoomTypesBuildings.Select(y => y.BuildingId).ToList()))
                .ReverseMap()
                .ForMember(x => x.RoomTypesBuildings, opt => opt.MapFrom(x => x.Buildings.Select(y => new RoomTypeBuilding() { BuildingId = y, RoomTypeId = x.Id }).ToList()));
            CreateMap<RoomType, RoomTypeDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
        }
    }
}
