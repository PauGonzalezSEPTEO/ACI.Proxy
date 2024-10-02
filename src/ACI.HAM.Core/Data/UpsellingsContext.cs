using ACI.HAM.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;

namespace ACI.HAM.Core.Data
{
    public class BaseContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public BaseContext(DbContextOptions<BaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoleTranslation> AspNetRoleTranslations { get; set; }

        public virtual DbSet<AuditEntry> AuditEntries { get; set; }

        public virtual DbSet<BoardHotelCompany> BoardHotelsCompanies { get; set; }

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
            var userHotels = GetUserHotels();
            return Boards
                .IgnoreQueryFilters()
                .Where(x => x.BoardHotelsCompanies.Any(x => userCompanies.Contains(x.CompanyId) && (!x.HotelId.HasValue || userHotels.Contains(x.HotelId.Value))))
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
                    .Include(x => x.UserHotelsCompanies)
                    .FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    companies = user.UserHotelsCompanies.Select(x => x.CompanyId).Distinct().ToList();
                }
            }
            return companies;
        }

        private List<int> GetUserHotels()
        {
            List<int> hotels = new List<int>();
            var currentUser = GetUser();
            if (currentUser != null)
            {
                var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = Users
                    .IgnoreQueryFilters()
                    .Include(x => x.UserHotelsCompanies)
                    .ThenInclude(x => x.Company)
                    .ThenInclude(x => x.Hotels)
                    .FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    var userHotelsCompanies = user.UserHotelsCompanies;
                    var allHotels = userHotelsCompanies
                        .Where(x => x.HotelId == null)
                        .SelectMany(x => x.Company.Hotels.Select(x => x.Id))
                        .Distinct()
                        .ToList();
                    var specificHotels = userHotelsCompanies
                        .Where(x => x.HotelId != null)
                        .Select(x => x.HotelId.Value)
                        .Distinct()
                        .ToList();
                    hotels = allHotels.Union(specificHotels).ToList();
                }
            }
            return hotels;
        }

        public Func<string> GetUserName { get; set; }

        private List<int> GetUserRoomTypes()
        {
            var userCompanies = GetUserCompanies();
            var userHotels = GetUserHotels();
            return RoomTypes
                .IgnoreQueryFilters()
                .Where(x => x.RoomTypeHotelsCompanies.Any(x => userCompanies.Contains(x.CompanyId) && (!x.HotelId.HasValue || userHotels.Contains(x.HotelId.Value))))
                .Select(x => x.Id)
                .ToList();
        }

        public virtual DbSet<Hotel> Hotels { get; set; }

        private bool IsAdministrator()
        {
            var currentUser = GetUser();
            return currentUser != null && !currentUser.IsInRole("Administrator");
        }

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
            modelBuilder.Entity<UserHotelCompany>(userHotelCompany =>
            {
                userHotelCompany.HasKey(e => new { e.Id });
                userHotelCompany.HasOne(e => e.User)
                    .WithMany(r => r.UserHotelsCompanies)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();
                userHotelCompany.HasOne(e => e.Company)
                    .WithMany(r => r.UserHotelsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                userHotelCompany.HasOne(e => e.Hotel)
                    .WithMany(r => r.UserHotelsCompanies)
                    .HasForeignKey(e => e.HotelId)
                    .IsRequired(false);
                userHotelCompany.HasIndex(e => new { e.UserId, e.CompanyId, e.HotelId })
                    .IsUnique()
                    .HasDatabaseName("UQ_UserHotelCompany");
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
            modelBuilder.Entity<BoardHotelCompany>(boardHotelCompany =>
            {
                boardHotelCompany.HasKey(e => new { e.Id });
                boardHotelCompany.HasOne(e => e.Board)
                    .WithMany(r => r.BoardHotelsCompanies)
                    .HasForeignKey(e => e.BoardId)
                    .IsRequired();
                boardHotelCompany.HasOne(e => e.Company)
                    .WithMany(r => r.BoardHotelsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                boardHotelCompany.HasOne(e => e.Hotel)
                    .WithMany(r => r.BoardHotelsCompanies)
                    .HasForeignKey(e => e.HotelId)
                    .IsRequired(false);
                boardHotelCompany.HasIndex(e => new { e.BoardId, e.CompanyId, e.HotelId })
                    .IsUnique()
                    .HasDatabaseName("UQ_BoardHotelCompany");
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
            modelBuilder.Entity<RoomTypeHotelCompany>(RoomTypeHotelCompany =>
            {
                RoomTypeHotelCompany.HasKey(e => new { e.Id });
                RoomTypeHotelCompany.HasOne(e => e.RoomType)
                    .WithMany(r => r.RoomTypeHotelsCompanies)
                    .HasForeignKey(e => e.RoomTypeId)
                    .IsRequired();
                RoomTypeHotelCompany.HasOne(e => e.Company)
                    .WithMany(r => r.RoomTypeHotelsCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                RoomTypeHotelCompany.HasOne(e => e.Hotel)
                    .WithMany(r => r.RoomTypeHotelsCompanies)
                    .HasForeignKey(e => e.HotelId)
                    .IsRequired(false);
                RoomTypeHotelCompany.HasIndex(e => new { e.RoomTypeId, e.CompanyId, e.HotelId })
                    .IsUnique()
                    .HasDatabaseName("UQ_RoomTypeHotelCompany");
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
            modelBuilder.Entity<Board>().HasQueryFilter(x => !IsAdministrator() || GetUserBoards().Contains(x.Id));
            modelBuilder.Entity<Company>().HasQueryFilter(x => !IsAdministrator() || GetUserCompanies().Contains(x.Id));
            modelBuilder.Entity<Hotel>().HasQueryFilter(x => !IsAdministrator() || GetUserHotels().Contains(x.Id));
            modelBuilder.Entity<RoomType>().HasQueryFilter(x => !IsAdministrator() || GetUserRoomTypes().Contains(x.Id));
        }

        public virtual DbSet<RoomTypeHotelCompany> RoomTypeHotelsCompanies { get; set; }

        public virtual DbSet<RoomType> RoomTypes { get; set; }

        public virtual DbSet<RoomTypeBuilding> RoomTypesBuildings { get; set; }

        public virtual DbSet<RoomTypeTranslation> RoomTypeTranslations { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await OnBeforeSaveChangesAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public virtual DbSet<UserApiKey> UserApiKeys { get; set; }

        public virtual DbSet<UserHotelCompany> UserHotelsCompanies { get; set; }
    }
}
