using ACI.Base.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace ACI.Base.Core.Data
{
    public class BaseContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public BaseContext(DbContextOptions<BaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoleTranslation> AspNetRoleTranslations { get; set; }

        public virtual DbSet<AuditEntry> AuditEntries { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<BoardBuilding> BoardsBuildings { get; set; }

        public virtual DbSet<BoardTranslation> BoardTranslations { get; set; }

        public virtual DbSet<Building> Buildings { get; set; }

        public virtual DbSet<BuildingTranslation> BuildingTranslations { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public Func<string> GetUserName { get; set; }

        public virtual DbSet<Hotel> Hotels { get; set; }

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
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");
            modelBuilder.Entity<UserCompany>(userCompany =>
            {
                userCompany.HasKey(e => new { e.UserId, e.CompanyId });
                userCompany.HasOne(e => e.Company)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.CompanyId)
                    .IsRequired();
                userCompany.HasOne(e => e.User)
                    .WithMany(r => r.Companies)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();
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
        }

        public virtual DbSet<RoomType> RoomTypes { get; set; }

        public virtual DbSet<RoomTypeBuilding> RoomTypesBuildings { get; set; }

        public virtual DbSet<RoomTypeTranslation> RoomTypeTranslations { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await OnBeforeSaveChangesAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public virtual DbSet<UserCompany> UserCompanies { get; set; }
    }
}
