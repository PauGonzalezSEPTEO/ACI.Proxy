using ACI.Proxy.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;

namespace ACI.Proxy.Core.Data
{
    public class BaseContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public BaseContext(DbContextOptions<BaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoleTranslation> AspNetRoleTranslations { get; set; }

        public virtual DbSet<AuditEntry> AuditEntries { get; set; }

        public virtual DbSet<BoardProjectCompany> BoardProjectsCompanies { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<BoardBuilding> BoardsBuildings { get; set; }

        public virtual DbSet<BoardTranslation> BoardTranslations { get; set; }

        public virtual DbSet<Building> Buildings { get; set; }

        public virtual DbSet<BuildingTranslation> BuildingTranslations { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public Func<ClaimsPrincipal> GetUser { get; set; }

        private List<int> GetUserBoards()
        {
            var userCompanies = GetUserCompanies();
            var userProjects = GetUserProjects();
            return Boards
                .IgnoreQueryFilters()
                .Where(x => x.BoardProjectsCompanies.Any(x => userCompanies.Contains(x.CompanyId) && (!x.ProjectId.HasValue || userProjects.Contains(x.ProjectId.Value))))
                .Select(x => x.Id)
                .ToList();
        }

        private List<int> GetUserCompanies()
        {
            List<int> companies = new List<int>();
            var currentUser = GetUser();
            if (currentUser != null)
            {
                var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = Users
                    .IgnoreQueryFilters()
                    .Include(x => x.UserProjectsCompanies)
                    .FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    companies = user.UserProjectsCompanies.Select(x => x.CompanyId).Distinct().ToList();
                }
            }
            return companies;
        }

        private List<int> GetUserProjects()
        {
            List<int> projects = new List<int>();
            var currentUser = GetUser();
            if (currentUser != null)
            {
                var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = Users
                    .IgnoreQueryFilters()
                    .Include(x => x.UserProjectsCompanies)
                    .ThenInclude(x => x.Company)
                    .ThenInclude(x => x.Projects)
                    .FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    var userProjectsCompanies = user.UserProjectsCompanies;
                    var allProjects = userProjectsCompanies
                        .Where(x => x.ProjectId == null)
                        .SelectMany(x => x.Company.Projects.Select(x => x.Id))
                        .Distinct()
                        .ToList();
                    var specificProjects = userProjectsCompanies
                        .Where(x => x.ProjectId != null)
                        .Select(x => x.ProjectId.Value)
                        .Distinct()
                        .ToList();
                    projects = allProjects.Union(specificProjects).ToList();
                }
            }
            return projects;
        }

        private string GetUserId()
        {
            var currentUser = GetUser();
            if (currentUser != null)
            {
                return currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return null;
        }

        public Func<string> GetUserName { get; set; }

        private List<int> GetUserRoomTypes()
        {
            var userCompanies = GetUserCompanies();
            var userProjects = GetUserProjects();
            return RoomTypes
                .IgnoreQueryFilters()
                .Where(x => x.RoomTypeProjectsCompanies.Any(x => userCompanies.Contains(x.CompanyId) && (!x.ProjectId.HasValue || userProjects.Contains(x.ProjectId.Value))))
                .Select(x => x.Id)
                .ToList();
        }

        private List<int> GetUserTemplates()
        {
            var userCompanies = GetUserCompanies();
            var userProjects = GetUserProjects();
            return Templates
                .IgnoreQueryFilters()
                .Where(x => x.TemplateProjectsCompanies.Any(x => userCompanies.Contains(x.CompanyId) && (!x.ProjectId.HasValue || userProjects.Contains(x.ProjectId.Value))))
                .Select(x => x.Id)
                .ToList();
        }

        private List<int> GetUserUserApiKeys()
        {
            var userId = GetUserId();
            return UserApiKeys
                .IgnoreQueryFilters()
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToList();
        }

        private List<int> GetUserUserApiUsageStatistics()
        {
            var userId = GetUserId();
            return UserApiUsageStatistics
                .IgnoreQueryFilters()
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToList();
        }

        public virtual DbSet<Integration> Integrations { get; set; }

        public virtual DbSet<IntegrationTranslation> IntegrationTranslations { get; set; }

        private bool IsAdministrator()
        {
            var currentUser = GetUser();
            return currentUser != null && !currentUser.IsInRole("Administrator");
        }

        public bool IsApiKeyRequest { get; set; }

        private async Task OnBeforeSaveChangesAsync()
        {
            ChangeTracker.DetectChanges();
            DateTime now = DateTime.UtcNow;
            List<AuditEntry> auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is IAuditable))
                {
                    continue;
                }
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                Dictionary<string, object> newValues = new Dictionary<string, object>();
                Dictionary<string, object> oldValues = new Dictionary<string, object>();
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        keyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue;
                                newValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
                AuditEntry auditEntry = new AuditEntry
                {
                    ActionType = entry.State == EntityState.Added ? ActionTypes.Insert : entry.State == EntityState.Deleted ? ActionTypes.Delete : ActionTypes.Update,
                    EntityName = entry.Metadata.ClrType.Name,
                    KeyValues = JsonConvert.SerializeObject(keyValues),
                    NewValues = newValues.Count == 0 ? null : JsonConvert.SerializeObject(newValues),
                    OldValues = oldValues.Count == 0 ? null : JsonConvert.SerializeObject(oldValues),
                    UserName = GetUserName(),
                    Timestamp = now
                };
                auditEntries.Add(auditEntry);
            }
            if (auditEntries.Count > 0)
            {
                await AuditEntries.AddRangeAsync(auditEntries);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("ProductVersion", "1.0.0-servicing-10000");
            modelBuilder.Entity<UserProjectCompany>(userProjectCompany =>
            {
                userProjectCompany.HasKey(e => new { e.Id });
                userProjectCompany.HasOne(e => e.User)
                    .WithMany(r => r.UserProjectsCompanies)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();
                userProjectCompany.HasOne(e => e.Company)
                    .WithMany(r => r.UserProjectsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                userProjectCompany.HasOne(e => e.Project)
                    .WithMany(r => r.UserProjectsCompanies)
                    .HasForeignKey(e => e.ProjectId)
                    .IsRequired(false);
                userProjectCompany.HasIndex(e => new { e.UserId, e.CompanyId, e.ProjectId })
                    .IsUnique()
                    .HasDatabaseName("UQ_UserProjectCompany");
            });
            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(e => new { e.UserId, e.RoleId });
                userRole.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .IsRequired();
                userRole.HasOne(e => e.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Role)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AspNetRoleTranslations_AspNetRoles");

            });
            modelBuilder.Entity<RoleTranslation>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.HasKey(e => new
                {
                    e.RoleId,
                    e.LanguageCode
                });
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.Translations)
                    .HasForeignKey(e => e.RoleId);
            });
            modelBuilder.Entity<BoardProjectCompany>(boardProjectCompany =>
            {
                boardProjectCompany.HasKey(e => new { e.Id });
                boardProjectCompany.HasOne(e => e.Board)
                    .WithMany(r => r.BoardProjectsCompanies)
                    .HasForeignKey(e => e.BoardId)
                    .IsRequired();
                boardProjectCompany.HasOne(e => e.Company)
                    .WithMany(r => r.BoardProjectsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                boardProjectCompany.HasOne(e => e.Project)
                    .WithMany(r => r.BoardProjectsCompanies)
                    .HasForeignKey(e => e.ProjectId)
                    .IsRequired(false);
                boardProjectCompany.HasIndex(e => new { e.BoardId, e.CompanyId, e.ProjectId })
                    .IsUnique()
                    .HasDatabaseName("UQ_BoardProjectCompany");
            });
            modelBuilder.Entity<Board>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Board)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BoardTranslations_Boards");
            });
            modelBuilder.Entity<BoardBuilding>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.BoardId,
                    e.BuildingId
                });
                entity.HasOne(e => e.Board)
                    .WithMany(e => e.BoardsBuildings)
                    .HasForeignKey(e => e.BoardId);
                entity.HasOne(e => e.Building)
                    .WithMany(e => e.BoardsBuildings)
                    .HasForeignKey(e => e.BuildingId);
            });
            modelBuilder.Entity<BoardTranslation>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });
            modelBuilder.Entity<Building>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Building)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BuildingTranslations_Buildings");

            });
            modelBuilder.Entity<BuildingTranslation>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.BuildingId,
                    e.LanguageCode
                });
                entity.HasOne(e => e.Building)
                    .WithMany(e => e.Translations)
                    .HasForeignKey(e => e.BuildingId);
            });
            modelBuilder.Entity<Integration>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Integration)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_IntegrationTranslations_Integrations");

            });
            modelBuilder.Entity<IntegrationTranslation>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.IntegrationId,
                    e.LanguageCode
                });
                entity.HasOne(e => e.Integration)
                    .WithMany(e => e.Translations)
                    .HasForeignKey(e => e.IntegrationId);
            });
            modelBuilder.Entity<RoomTypeProjectCompany>(RoomTypeProjectCompany =>
            {
                RoomTypeProjectCompany.HasKey(e => new { e.Id });
                RoomTypeProjectCompany.HasOne(e => e.RoomType)
                    .WithMany(r => r.RoomTypeProjectsCompanies)
                    .HasForeignKey(e => e.RoomTypeId)
                    .IsRequired();
                RoomTypeProjectCompany.HasOne(e => e.Company)
                    .WithMany(r => r.RoomTypeProjectsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                RoomTypeProjectCompany.HasOne(e => e.Project)
                    .WithMany(r => r.RoomTypeProjectsCompanies)
                    .HasForeignKey(e => e.ProjectId)
                    .IsRequired(false);
                RoomTypeProjectCompany.HasIndex(e => new { e.RoomTypeId, e.CompanyId, e.ProjectId })
                    .IsUnique()
                    .HasDatabaseName("UQ_RoomTypeProjectCompany");
            });
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.RoomType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoomTypeTranslations_RoomTypes");
            });
            modelBuilder.Entity<RoomTypeBuilding>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.RoomTypeId,
                    e.BuildingId
                });
                entity.HasOne(e => e.RoomType)
                    .WithMany(e => e.RoomTypesBuildings)
                    .HasForeignKey(e => e.RoomTypeId);
                entity.HasOne(e => e.Building)
                    .WithMany(e => e.RoomTypesBuildings)
                    .HasForeignKey(e => e.BuildingId);
            });
            modelBuilder.Entity<RoomTypeTranslation>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });
            modelBuilder.Entity<RoomTypeTranslation>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.RoomTypeId,
                    e.LanguageCode
                });
                entity.HasOne(e => e.RoomType)
                    .WithMany(e => e.Translations)
                    .HasForeignKey(e => e.RoomTypeId);
            });
            modelBuilder.Entity<TemplateProjectCompany>(templateProjectCompany =>
            {
                templateProjectCompany.HasKey(e => new { e.Id });
                templateProjectCompany.HasOne(e => e.Template)
                    .WithMany(r => r.TemplateProjectsCompanies)
                    .HasForeignKey(e => e.TemplateId)
                    .IsRequired();
                templateProjectCompany.HasOne(e => e.Company)
                    .WithMany(r => r.TemplateProjectsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                templateProjectCompany.HasOne(e => e.Project)
                    .WithMany(r => r.TemplateProjectsCompanies)
                    .HasForeignKey(e => e.ProjectId)
                    .IsRequired(false);
                templateProjectCompany.HasIndex(e => new { e.TemplateId, e.CompanyId, e.ProjectId })
                    .IsUnique()
                    .HasDatabaseName("UQ_TemplateProjectCompany");
            });
            modelBuilder.Entity<Template>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.ShortDescription).IsUnicode(false);
                entity.HasMany(e => e.Translations)
                    .WithOne(e => e.Template)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TemplateTranslations_Templates");
            });
            modelBuilder.Entity<TemplateBuilding>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.TemplateId,
                    e.BuildingId
                });
                entity.HasOne(e => e.Template)
                    .WithMany(e => e.TemplatesBuildings)
                    .HasForeignKey(e => e.TemplateId);
                entity.HasOne(e => e.Building)
                    .WithMany(e => e.TemplatesBuildings)
                    .HasForeignKey(e => e.BuildingId);
            });
            modelBuilder.Entity<TemplateTranslation>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });
            modelBuilder.Entity<Board>().HasQueryFilter(x => !IsAdministrator() || GetUserBoards().Contains(x.Id));
            modelBuilder.Entity<Company>().HasQueryFilter(x => !IsAdministrator() || GetUserCompanies().Contains(x.Id));
            modelBuilder.Entity<Project>().HasQueryFilter(x => !IsAdministrator() || GetUserProjects().Contains(x.Id));
            modelBuilder.Entity<RoomType>().HasQueryFilter(x => !IsAdministrator() || GetUserRoomTypes().Contains(x.Id));
            modelBuilder.Entity<Template>().HasQueryFilter(x => !IsAdministrator() || GetUserTemplates().Contains(x.Id));
            modelBuilder.Entity<UserApiKey>().HasQueryFilter(x => IsApiKeyRequest || !IsAdministrator() || GetUserUserApiKeys().Contains(x.Id));
            modelBuilder.Entity<UserApiUsageStatistic>().HasQueryFilter(x => IsApiKeyRequest || !IsAdministrator() || GetUserUserApiUsageStatistics().Contains(x.Id));
        }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<RoomTypeProjectCompany> RoomTypeProjectsCompanies { get; set; }

        public virtual DbSet<RoomType> RoomTypes { get; set; }

        public virtual DbSet<RoomTypeBuilding> RoomTypesBuildings { get; set; }

        public virtual DbSet<RoomTypeTranslation> RoomTypeTranslations { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await OnBeforeSaveChangesAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public virtual DbSet<TemplateProjectCompany> TemplateProjectsCompanies { get; set; }

        public virtual DbSet<Template> Templates { get; set; }

        public virtual DbSet<TemplateBuilding> TemplatesBuildings { get; set; }

        public virtual DbSet<TemplateTranslation> TemplateTranslations { get; set; }

        public virtual DbSet<UserApiKey> UserApiKeys { get; set; }

        public virtual DbSet<UserApiUsageStatistic> UserApiUsageStatistics { get; set; }

        public virtual DbSet<UserProjectCompany> UserProjectsCompanies { get; set; }
    }
}
