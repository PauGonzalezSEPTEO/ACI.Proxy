CREATE TABLE [dbo].[Test](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)) ON [PRIMARY]
GO

INSERT INTO [Test] ([Type])
VALUES ('Hatchback'), ('Kombi'), ('Sedan')
GO

GRANT ALL ON Test TO [user]
GO
