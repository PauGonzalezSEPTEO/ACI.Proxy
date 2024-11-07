using System.Linq;
using AutoMapper;
using ACI.Proxy.Core.Dtos;
using ACI.Proxy.Core.Models;

namespace ACI.Proxy.Core.Profiles
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
                .ForMember(x => x.UserProjectsCompanies, opt => opt.MapFrom(x => x.UserProjectsCompanies.Select(y => new UserProjectCompany() { CompanyId = y.CompanyId, ProjectId = y.ProjectId }).ToList()));
            CreateMap<RegistrationDto, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email));
            CreateMap<User, AccountDto>().ReverseMap();
            CreateMap<ProfileDetailsDto, User>();
            CreateMap<UserApiKey, UserApiKeyDto>()
                .ReverseMap();
            CreateMap<UserProjectCompany, UserProjectCompanyDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name))
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project != null ? x.Project.Name : null))
                .ReverseMap();
            CreateMap<User, UserEditableDto>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(x => x.UserRoles.Select(y => y.RoleId).ToList()))
                .ReverseMap();
            CreateMap<UserEditableDto, UpdatableUser>()
                .ForMember(x => x.UserRoles, opt => opt.MapFrom(x => x.Roles.Select(y => new UserRole() { UserId = x.Id, RoleId = y }).ToList()))
                .ForMember(x => x.UserProjectsCompanies, opt => opt.MapFrom(x => x.UserProjectsCompanies.Select(y => new UserProjectCompany() { UserId = x.Id, CompanyId = y.CompanyId, ProjectId = y.ProjectId }).ToList()));
            CreateMap<User, UserDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Roles, opt => opt.MapFrom(x => x.UserRoles.Select(y => y.Role)))
                .ForMember(x => x.UserProjectsCompanies, opt => opt.MapFrom(x => x.UserProjectsCompanies.Select(y => new UserProjectCompanyDto
                 {
                     CompanyId = y.CompanyId,
                     ProjectId = y.ProjectId,
                     CompanyName = y.Company.Name,
                     ProjectName = y.Project != null ? y.Project.Name : null
                 }).ToList()));
            CreateMap<CompanyEditableDto, Company>().ReverseMap();
            CreateMap<Company, CompanyDto>();
            CreateMap<ProjectEditableDto, Project>().ReverseMap();
            CreateMap<Project, ProjectDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name));
            CreateMap<BoardTranslationDto, BoardTranslation>()
                .ReverseMap();
            CreateMap<BoardProjectCompany, BoardProjectCompanyDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name))
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project != null ? x.Project.Name : null))
                .ReverseMap();
            CreateMap<Board, BoardEditableDto>()
                .ForMember(x => x.Buildings, opt => opt.MapFrom(x => x.BoardsBuildings.Select(y => y.BuildingId).ToList()))
                .ReverseMap()
                .ForMember(x => x.BoardsBuildings, opt => opt.MapFrom(x => x.Buildings.Select(y => new BoardBuilding() { BuildingId = y, BoardId = x.Id }).ToList()))
                .ForMember(x => x.BoardProjectsCompanies, opt => opt.MapFrom(x => x.BoardProjectsCompanies.Select(y => new BoardProjectCompany() { BoardId = x.Id, CompanyId = y.CompanyId, ProjectId = y.ProjectId }).ToList()));
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
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project.Name));
            CreateMap<Building, BuildingEditableDto>()
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
            CreateMap<IntegrationTranslationDto, IntegrationTranslation>()
                .ReverseMap();
            CreateMap<IntegrationEditableDto, Integration>()
                .ReverseMap();
            CreateMap<Integration, IntegrationDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations))
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project.Name));
            CreateMap<Integration, IntegrationEditableDto>()
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
            CreateMap<RoomTypeTranslationDto, RoomTypeTranslation>()
                .ReverseMap();
            CreateMap<RoomTypeProjectCompany, RoomTypeProjectCompanyDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name))
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project != null ? x.Project.Name : null))
                .ReverseMap();
            CreateMap<RoomType, RoomTypeEditableDto>()
                .ForMember(x => x.Buildings, opt => opt.MapFrom(x => x.RoomTypesBuildings.Select(y => y.BuildingId).ToList()))
                .ReverseMap()
                .ForMember(x => x.RoomTypesBuildings, opt => opt.MapFrom(x => x.Buildings.Select(y => new RoomTypeBuilding() { BuildingId = y, RoomTypeId = x.Id }).ToList()))
                .ForMember(x => x.RoomTypeProjectsCompanies, opt => opt.MapFrom(x => x.RoomTypeProjectsCompanies.Select(y => new RoomTypeProjectCompany() { RoomTypeId = x.Id, CompanyId = y.CompanyId, ProjectId = y.ProjectId }).ToList()));
            CreateMap<RoomType, RoomTypeDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
            CreateMap<TemplateTranslationDto, TemplateTranslation>()
                .ReverseMap();
            CreateMap<TemplateProjectCompany, TemplateProjectCompanyDto>()
                .ForMember(x => x.CompanyName, opt => opt.MapFrom(x => x.Company.Name))
                .ForMember(x => x.ProjectName, opt => opt.MapFrom(x => x.Project != null ? x.Project.Name : null))
                .ReverseMap();
            CreateMap<Template, TemplateEditableDto>()
                .ForMember(x => x.Buildings, opt => opt.MapFrom(x => x.TemplatesBuildings.Select(y => y.BuildingId).ToList()))
                .ReverseMap()
                .ForMember(x => x.TemplatesBuildings, opt => opt.MapFrom(x => x.Buildings.Select(y => new TemplateBuilding() { BuildingId = y, TemplateId = x.Id }).ToList()))
                .ForMember(x => x.TemplateProjectsCompanies, opt => opt.MapFrom(x => x.TemplateProjectsCompanies.Select(y => new TemplateProjectCompany() { TemplateId = x.Id, CompanyId = y.CompanyId, ProjectId = y.ProjectId }).ToList()));
            CreateMap<Template, TemplateDto>()
                .ForCtorParam("languageCode", opt => opt.MapFrom(x => languageCode))
                .ForMember(x => x.Translations, opt => opt.MapFrom(x => x.Translations));
        }
    }
}
