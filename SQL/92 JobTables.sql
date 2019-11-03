
/****** Object:  Table [dbo].[JobTables]    Script Date: 9/15/2019 12:06:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobTables](
	[TableRno] [int] NOT NULL IDENTITY (1, 1),
	[JobRno] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
	[Caterer] [varchar](60) NULL,
	[Other] [varchar](60) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.JobTables ADD CONSTRAINT
	PK_JobTables PRIMARY KEY CLUSTERED 
	(
	TableRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_JobTables_Jobs ON dbo.JobTables
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

INSERT [dbo].[JobTables] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 1, N'4 ft', NULL, NULL, CAST(N'2019-09-15T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobTables] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 2, N'6 ft', NULL, NULL, CAST(N'2019-09-15T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobTables] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 3, N'8 ft', NULL, NULL, CAST(N'2019-09-15T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO


