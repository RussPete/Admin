
/****** Object:  Table [dbo].[JobLinens]    Script Date: 9/15/2019 12:06:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JobLinens](
	[LinenRno] [int] NOT NULL IDENTITY (1, 1),
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
ALTER TABLE dbo.JobLinens ADD CONSTRAINT
	PK_JobLinens PRIMARY KEY CLUSTERED 
	(
	LinenRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_JobLinens_Jobs ON dbo.JobLinens
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 1, N'Base Color', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 2, N'Alsco', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 3, N'Alsco Floor', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 4, N'90 x 132', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 5, N'90 x 156', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 6, N'Rounds', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 7, N'Napkins', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
INSERT [dbo].[JobLinens] ([JobRno], [Seq], [Description], [Caterer], [Other], [CreatedDtTm], [CreatedUser], [UpdatedDtTm], [UpdatedUser]) VALUES (0, 8, N'Overlays', NULL, NULL, CAST(N'2019-09-17T00:00:00.000' AS DateTime), N'Sys', NULL, NULL)
GO
