USE [master]
GO

IF DB_ID('Proxy') IS NOT NULL
  set noexec on

CREATE DATABASE [Proxy];
GO

USE [Proxy]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[RoleId] [nvarchar](450) NOT NULL
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
  [ShortDescription] [varchar](500) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoleTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetRoleTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[ShortDescription] [varchar](500) NULL
 CONSTRAINT [PK_AspNetRoleTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [RoleId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [DATETIMEOFFSET](7) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
  [Firstname] [nvarchar](256) NOT NULL,
  [Lastname] [nvarchar](256) NOT NULL,
  [RefreshToken] [nvarchar](450) NULL,
  [RefreshTokenExpiryTime] [DATETIMEOFFSET](7) NULL,
  [AllowCommercialMail] [bit] NULL,
  [Avatar] [nvarchar](max) NULL,
  [CommunicationByMail] [bit] NULL,
  [CommunicationByPhone] [bit] NULL,
  [CompanyName] [nvarchar](256) NULL,
  [CreateDate] [DATETIMEOFFSET](7) NULL,
  [CurrencyCode] [nvarchar](3) NULL,
  [CountryAlpha2Code] [nvarchar](2) NULL,
  [LanguageAlpha2Code] [nvarchar](2) NULL,
  [LastLoginDate] [DATETIMEOFFSET](7) NULL
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetRoleClaims_AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]'))
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetRoleClaims_AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]'))
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserClaims_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]'))
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserClaims_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]'))
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserLogins_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]'))
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserLogins_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]'))
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserTokens_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]'))
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetUserTokens_AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]'))
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetRoleTranslations_AspNetRoles]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetRoleTranslations]'))
ALTER TABLE [dbo].[AspNetRoleTranslations]  WITH CHECK ADD CONSTRAINT [FK_AspNetRoleTranslations_AspNetRoles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AspNetRoleTranslations_AspNetRoles]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetRoleTranslations]'))
ALTER TABLE [dbo].[AspNetRoleTranslations] CHECK CONSTRAINT [FK_AspNetRoleTranslations_AspNetRoles]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditEntries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AuditEntries](
  [ActionType] [int] NOT NULL,
	[EntityName] [nvarchar](MAX) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
  [KeyValues][nvarchar](MAX) NOT NULL,
  [NewValues][nvarchar](MAX) NULL,
  [OldValues][nvarchar](MAX) NULL,
  [Timestamp] [DATETIMEOFFSET](7) NOT NULL,
  [UserName] [nvarchar](256) NULL,
 CONSTRAINT [PK_AuditEntries] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserApiUsageStatistics]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserApiUsageStatistics] (
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [nvarchar](450) NOT NULL,
    [ApiRoute] [varchar](512) NOT NULL,
    [RequestTime] [DATETIMEOFFSET] (7) NOT NULL
 CONSTRAINT [PK_UserApiUsageStatistics] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Companies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Companies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Projects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
  [CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserApiKeys]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserApiKeys](
  Id INT IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
  [ApiKeyLast6] NVARCHAR(6) NOT NULL,
  [EncryptedApiKey] NVARCHAR(512) NOT NULL,
  [HashedApiKey] NVARCHAR(64) NOT NULL,
  [Salt] NVARCHAR(32) NOT NULL,
  [CreatedAt] [DATETIMEOFFSET] (7) NOT NULL,
  [Expiration] [DATETIMEOFFSET] (7) NOT NULL,
  [IsActive] BIT NOT NULL
 CONSTRAINT [PK_UserApiKeys] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProjectsCompanies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserProjectsCompanies](
  Id INT IDENTITY(1,1) PRIMARY KEY,
	[UserId] [nvarchar](450) NOT NULL,
	[CompanyId] [int] NOT NULL,
  [ProjectId] [int] NULL
 CONSTRAINT UQ_UserProjectCompany UNIQUE ([UserId], [CompanyId], [ProjectId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RoomTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[ShortDescription] [varchar](500) NULL
 CONSTRAINT [PK_RoomTypes] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypeProjectsCompanies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RoomTypeProjectsCompanies](
  Id INT IDENTITY(1,1) PRIMARY KEY,
	[RoomTypeId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
  [ProjectId] [int] NULL
 CONSTRAINT UQ_RoomTypeProjectCompany UNIQUE ([RoomTypeId], [CompanyId], [ProjectId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypeTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RoomTypeTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[RoomTypeId] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortDescription] [varchar](500) NULL,
 CONSTRAINT [PK_RoomTypeTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [RoomTypeId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Boards]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Boards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[ShortDescription] [varchar](500) NULL
 CONSTRAINT [PK_Boards] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BoardProjectsCompanies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BoardProjectsCompanies](
  Id INT IDENTITY(1,1) PRIMARY KEY,
	[BoardId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
  [ProjectId] [int] NULL
 CONSTRAINT UQ_BoardProjectCompany UNIQUE ([BoardId], [CompanyId], [ProjectId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BoardTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BoardTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[BoardId] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortDescription] [varchar](500) NULL,
 CONSTRAINT [PK_BoardTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [BoardId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Buildings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[ShortDescription] [varchar](500) NULL,
  [ProjectId] [int] NOT NULL,
 CONSTRAINT [PK_Buildings] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BuildingTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BuildingTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[BuildingId] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortDescription] [varchar](500) NULL,
 CONSTRAINT [PK_BuildingTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [BuildingId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Integrations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Integrations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[ShortDescription] [varchar](500) NULL,
  [ProjectId] [int] NOT NULL,
 CONSTRAINT [PK_Integrations] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IntegrationTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[IntegrationTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[IntegrationId] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortDescription] [varchar](500) NULL,
 CONSTRAINT [PK_IntegrationTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [IntegrationId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypesBuildings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RoomTypesBuildings](
	[RoomTypeId] [int] NOT NULL,
  [BuildingId] [int] NOT NULL,
 CONSTRAINT [PK_RoomTypesBuildings] PRIMARY KEY CLUSTERED
(
	[RoomTypeId] ASC,
  [BuildingId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BoardsBuildings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BoardsBuildings](
	[BoardId] [int] NOT NULL,
  [BuildingId] [int] NOT NULL,
 CONSTRAINT [PK_BoardsBuildings] PRIMARY KEY CLUSTERED
(
	[BoardId] ASC,
  [BuildingId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplatesBuildings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TemplatesBuildings](
	[TemplateId] [int] NOT NULL,
  [BuildingId] [int] NOT NULL,
 CONSTRAINT [PK_TemplatesBuildings] PRIMARY KEY CLUSTERED
(
	[TemplateId] ASC,
  [BuildingId] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Templates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Templates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NULL,
	[ShortDescription] [varchar](500) NULL,
  [Content] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplateProjectsCompanies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TemplateProjectsCompanies](
  Id INT IDENTITY(1,1) PRIMARY KEY,
	[TemplateId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
  [ProjectId] [int] NULL
 CONSTRAINT UQ_TemplateProjectCompany UNIQUE ([TemplateId], [CompanyId], [ProjectId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
)) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplateTranslations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TemplateTranslations](
	[LanguageCode] [varchar](10) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ShortDescription] [varchar](500) NULL,
  [Content] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_TemplateTranslations] PRIMARY KEY CLUSTERED
(
	[LanguageCode] ASC,
  [TemplateId] ASC
)) ON [PRIMARY]
END
GO

SET IDENTITY_INSERT [dbo].[Companies] ON
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (1, N'Sun Projects')
GO
INSERT [dbo].[Companies] ([Id], [Name]) VALUES (2, N'Snow Projects')
GO
SET IDENTITY_INSERT [dbo].[Companies] OFF
GO

SET IDENTITY_INSERT [dbo].[Projects] ON
GO
INSERT [dbo].[Projects] ([Id], [Name], [CompanyId]) VALUES (1, N'Paradise Project', 1)
GO
INSERT [dbo].[Projects] ([Id], [Name], [CompanyId]) VALUES (2, N'Tropical Project', 1)
GO
INSERT [dbo].[Projects] ([Id], [Name], [CompanyId]) VALUES (3, N'Mountain Project', 2)
GO
INSERT [dbo].[Projects] ([Id], [Name], [CompanyId]) VALUES (4, N'White Project', 2)
GO
SET IDENTITY_INSERT [dbo].[Projects] OFF
GO

SET IDENTITY_INSERT [dbo].[RoomTypes] ON
GO
INSERT [dbo].[RoomTypes] ([Id], [Name], [ShortDescription]) VALUES (1, N'Double holiday with terrace', N'Room with terrace. Two Queen Size double beds (135cmx190cm). No extra beds available. Rooms with full bathroom with hairdryer, TV with foreign channels via Satellite, Refrigerator/Cooler, Room-Serving, Direct phone, Air conditioning and independent heating. Safe deposit box available (optional with supplement)')
GO
INSERT [dbo].[RoomTypes] ([Id], [Name]) VALUES (2, N'Single room')
GO
SET IDENTITY_INSERT [dbo].[RoomTypes] OFF
GO

INSERT [dbo].[RoomTypeProjectsCompanies] ([RoomTypeId], [CompanyId], [ProjectId]) VALUES (1, 1, null)
GO

INSERT [dbo].[RoomTypeProjectsCompanies] ([RoomTypeId], [CompanyId], [ProjectId]) VALUES (2, 2, 4)
GO

INSERT [dbo].[RoomTypeTranslations] ([LanguageCode], [RoomTypeId], [Name], [ShortDescription]) VALUES (N'es', 1, N'Habitación familiar', N'Ideal para parejas y familias de hasta cuatro miembros con dos camas queen size (1.35x1.90). Redecoradas en tonalidades claras entre 2019 y 2020, tienen una amplia terraza (sin vistas al mar). Baño completo con secador de pelo, TV de pantalla plana con canales extranjeros vía satélite, minibar / mantenedor de frío, room-service, aire acondicionado y calefacción independientes, terraza con mesa, sillas y tendedero, Wifi gratuito, caja de seguridad disponible (opcional con suplemento)')
GO

SET IDENTITY_INSERT [dbo].[Boards] ON
GO
INSERT [dbo].[Boards] ([Id], [Name]) VALUES (1, N'Half board')
GO
INSERT [dbo].[Boards] ([Id], [Name]) VALUES (2, N'Full board')
GO
SET IDENTITY_INSERT [dbo].[Boards] OFF
GO

INSERT [dbo].[BoardProjectsCompanies] ([BoardId], [CompanyId], [ProjectId]) VALUES (1, 1, null)
GO

INSERT [dbo].[BoardProjectsCompanies] ([BoardId], [CompanyId], [ProjectId]) VALUES (2, 2, 4)
GO

INSERT [dbo].[BoardTranslations] ([LanguageCode], [BoardId], [Name]) VALUES (N'es', 1, N'Media pensión')
GO

SET IDENTITY_INSERT [dbo].[Buildings] ON
GO
INSERT [dbo].[Buildings] ([Id], [Name], [ProjectId]) VALUES (1, N'Mediterranean', 1)
GO
SET IDENTITY_INSERT [dbo].[Buildings] OFF
GO

INSERT [dbo].[BuildingTranslations] ([LanguageCode], [BuildingId], [Name]) VALUES (N'es', 1, N'Mediterráneo')
GO

INSERT [dbo].[RoomTypesBuildings] ([RoomTypeId], [BuildingId]) VALUES (1, 1)
GO

ALTER TABLE [dbo].[UserApiUsageStatistics]  WITH CHECK ADD CONSTRAINT [FK_UserApiUsageStatistics_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserApiUsageStatistics] CHECK CONSTRAINT [FK_UserApiUsageStatistics_AspNetUsers]
GO

ALTER TABLE [dbo].[Projects]  WITH CHECK ADD CONSTRAINT [FK_Projects_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_Companies]
GO

ALTER TABLE [dbo].[UserApiKeys]  WITH CHECK ADD CONSTRAINT [FK_UserApiKeys_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserApiKeys] CHECK CONSTRAINT [FK_UserApiKeys_AspNetUsers]
GO

ALTER TABLE [dbo].[UserProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_UserProjectsCompanies_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserProjectsCompanies] CHECK CONSTRAINT [FK_UserProjectsCompanies_AspNetUsers]
GO

ALTER TABLE [dbo].[UserProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_UserProjectsCompanies_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserProjectsCompanies] CHECK CONSTRAINT [FK_UserProjectsCompanies_Companies]
GO

ALTER TABLE [dbo].[UserProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_UserProjectsCompanies_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[UserProjectsCompanies] CHECK CONSTRAINT [FK_UserProjectsCompanies_Projects]
GO

ALTER TABLE [dbo].[RoomTypeProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_RoomTypeProjectsCompanies_RoomTypes] FOREIGN KEY([RoomTypeId])
REFERENCES [dbo].[RoomTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoomTypeProjectsCompanies] CHECK CONSTRAINT [FK_RoomTypeProjectsCompanies_RoomTypes]
GO

ALTER TABLE [dbo].[RoomTypeProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_RoomTypeProjectsCompanies_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoomTypeProjectsCompanies] CHECK CONSTRAINT [FK_RoomTypeProjectsCompanies_Companies]
GO

ALTER TABLE [dbo].[RoomTypeProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_RoomTypeProjectsCompanies_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[RoomTypeProjectsCompanies] CHECK CONSTRAINT [FK_RoomTypeProjectsCompanies_Projects]
GO

ALTER TABLE [dbo].[RoomTypeTranslations]  WITH CHECK ADD CONSTRAINT [FK_RoomTypeTranslations_RoomTypes] FOREIGN KEY([RoomTypeId])
REFERENCES [dbo].[RoomTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoomTypeTranslations] CHECK CONSTRAINT [FK_RoomTypeTranslations_RoomTypes]
GO

ALTER TABLE [dbo].[BoardProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_BoardProjectsCompanies_Boards] FOREIGN KEY([BoardId])
REFERENCES [dbo].[Boards] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BoardProjectsCompanies] CHECK CONSTRAINT [FK_BoardProjectsCompanies_Boards]
GO

ALTER TABLE [dbo].[BoardProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_BoardProjectsCompanies_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BoardProjectsCompanies] CHECK CONSTRAINT [FK_BoardProjectsCompanies_Companies]
GO

ALTER TABLE [dbo].[BoardProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_BoardProjectsCompanies_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[BoardProjectsCompanies] CHECK CONSTRAINT [FK_BoardProjectsCompanies_Projects]
GO

ALTER TABLE [dbo].[BoardTranslations]  WITH CHECK ADD CONSTRAINT [FK_BoardTranslations_Boards] FOREIGN KEY([BoardId])
REFERENCES [dbo].[Boards] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BoardTranslations] CHECK CONSTRAINT [FK_BoardTranslations_Boards]
GO

ALTER TABLE [dbo].[Buildings]  WITH CHECK ADD CONSTRAINT [FK_Buildings_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Buildings] CHECK CONSTRAINT [FK_Buildings_Projects]
GO

ALTER TABLE [dbo].[BuildingTranslations]  WITH CHECK ADD CONSTRAINT [FK_BuildingTranslations_Buildings] FOREIGN KEY([BuildingId])
REFERENCES [dbo].[Buildings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BuildingTranslations] CHECK CONSTRAINT [FK_BuildingTranslations_Buildings]
GO

ALTER TABLE [dbo].[Integrations]  WITH CHECK ADD CONSTRAINT [FK_Integrations_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Integrations] CHECK CONSTRAINT [FK_Integrations_Projects]
GO

ALTER TABLE [dbo].[IntegrationTranslations]  WITH CHECK ADD CONSTRAINT [FK_IntegrationTranslations_Integrations] FOREIGN KEY([IntegrationId])
REFERENCES [dbo].[Integrations] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IntegrationTranslations] CHECK CONSTRAINT [FK_IntegrationTranslations_Integrations]
GO

ALTER TABLE [dbo].[RoomTypesBuildings]  WITH CHECK ADD CONSTRAINT [FK_RoomTypesBuildings_RoomTypes] FOREIGN KEY([RoomTypeId])
REFERENCES [dbo].[RoomTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoomTypesBuildings] CHECK CONSTRAINT [FK_RoomTypesBuildings_RoomTypes]
GO

ALTER TABLE [dbo].[RoomTypesBuildings]  WITH CHECK ADD CONSTRAINT [FK_RoomTypesBuildings_Buildings] FOREIGN KEY([BuildingId])
REFERENCES [dbo].[Buildings] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[RoomTypesBuildings] CHECK CONSTRAINT [FK_RoomTypesBuildings_Buildings]
GO

ALTER TABLE [dbo].[BoardsBuildings]  WITH CHECK ADD CONSTRAINT [FK_BoardsBuildings_Boards] FOREIGN KEY([BoardId])
REFERENCES [dbo].[Boards] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BoardsBuildings] CHECK CONSTRAINT [FK_BoardsBuildings_Boards]
GO

ALTER TABLE [dbo].[BoardsBuildings]  WITH CHECK ADD CONSTRAINT [FK_BoardsBuildings_Buildings] FOREIGN KEY([BuildingId])
REFERENCES [dbo].[Buildings] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[BoardsBuildings] CHECK CONSTRAINT [FK_BoardsBuildings_Buildings]
GO

ALTER TABLE [dbo].[TemplatesBuildings]  WITH CHECK ADD CONSTRAINT [FK_TemplatesBuildings_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemplatesBuildings] CHECK CONSTRAINT [FK_TemplatesBuildings_Templates]
GO

ALTER TABLE [dbo].[TemplatesBuildings]  WITH CHECK ADD CONSTRAINT [FK_TemplatesBuildings_Buildings] FOREIGN KEY([BuildingId])
REFERENCES [dbo].[Buildings] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[TemplatesBuildings] CHECK CONSTRAINT [FK_TemplatesBuildings_Buildings]
GO

ALTER TABLE [dbo].[TemplateProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_TemplateProjectsCompanies_Boards] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemplateProjectsCompanies] CHECK CONSTRAINT [FK_TemplateProjectsCompanies_Boards]
GO

ALTER TABLE [dbo].[TemplateProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_TemplateProjectsCompanies_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemplateProjectsCompanies] CHECK CONSTRAINT [FK_TemplateProjectsCompanies_Companies]
GO

ALTER TABLE [dbo].[TemplateProjectsCompanies]  WITH CHECK ADD CONSTRAINT [FK_TemplateProjectsCompanies_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])
ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[TemplateProjectsCompanies] CHECK CONSTRAINT [FK_TemplateProjectsCompanies_Projects]
GO

ALTER TABLE [dbo].[TemplateTranslations]  WITH CHECK ADD CONSTRAINT [FK_TemplateTranslations_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[Templates] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemplateTranslations] CHECK CONSTRAINT [FK_TemplateTranslations_Templates]
GO

IF NOT (EXISTS (SELECT * FROM sys.sysusers WHERE name='migrator'))
BEGIN
CREATE LOGIN [migrator] WITH PASSWORD = '.acisa159753'

CREATE USER [migrator] FOR LOGIN [migrator] WITH DEFAULT_SCHEMA=[dbo]

EXEC sp_addrolemember N'db_owner', N'migrator'
END

IF NOT (EXISTS (SELECT * FROM sys.sysusers WHERE name='user'))
BEGIN
CREATE LOGIN [user] WITH PASSWORD = '.acisa159753'

CREATE USER [user] FOR LOGIN [user] WITH DEFAULT_SCHEMA=[dbo]

GRANT ALL ON AuditEntries TO [user];
GRANT ALL ON Companies TO [user];
GRANT ALL ON Projects TO [user];
GRANT ALL ON RoomTypes TO [user];
GRANT ALL ON RoomTypeProjectsCompanies TO [user];
GRANT ALL ON RoomTypeTranslations TO [user];
GRANT ALL ON RoomTypesBuildings TO [user];
GRANT ALL ON Boards TO [user];
GRANT ALL ON BoardProjectsCompanies TO [user];
GRANT ALL ON BoardTranslations TO [user];
GRANT ALL ON BoardsBuildings TO [user];
GRANT ALL ON Buildings TO [user];
GRANT ALL ON BuildingTranslations TO [user];
GRANT ALL ON Integrations TO [user];
GRANT ALL ON IntegrationTranslations TO [user];
GRANT ALL ON Templates TO [user];
GRANT ALL ON TemplateProjectsCompanies TO [user];
GRANT ALL ON TemplateTranslations TO [user];
GRANT ALL ON TemplatesBuildings TO [user];
GRANT ALL ON UserApiKeys TO [user];
GRANT ALL ON UserApiUsageStatistics TO [user];
GRANT ALL ON UserProjectsCompanies TO [user];

END

GO
