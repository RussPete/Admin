/****** Object:  Table [dbo].[KitchenLocations]    Script Date: 11/27/2012 13:33:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[KitchenLocations](
	[KitchenLocRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[DefaultFlg] [bit] NULL,
	[HideFlg] [bit] NULL,
 CONSTRAINT [PK_KitchenLocations] PRIMARY KEY CLUSTERED 
(
	[KitchenLocRno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


BEGIN TRANSACTION
GO
insert into KitchenLocations (Name, SortOrder, DefaultFlg) values ('Hot Kitchen', 1, 1)
insert into KitchenLocations (Name, SortOrder, DefaultFlg) values ('Bakery', 2, 0)
insert into KitchenLocations (Name, SortOrder, DefaultFlg) values ('Packing Area', 3, 0)
GO
COMMIT


BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_mcJobMenuItems
	(
	MenuItemRno int NOT NULL IDENTITY (1, 1),
	Category varchar(50) NULL,
	MenuItem varchar(50) NULL,
	KitchenLocRno int NULL,
	HotFlg bit NULL,
	ColdFlg bit NULL,
	HideFlg bit NULL,
	MultSelFlg bit NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobMenuItems SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobMenuItems ON
GO
IF EXISTS(SELECT * FROM dbo.mcJobMenuItems)
	 EXEC('INSERT INTO dbo.Tmp_mcJobMenuItems (MenuItemRno, Category, MenuItem, HotFlg, ColdFlg, HideFlg, MultSelFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT MenuItemRno, Category, MenuItem, HotFlg, ColdFlg, HideFlg, MultSelFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobMenuItems WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobMenuItems OFF
GO
DROP TABLE dbo.mcJobMenuItems
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobMenuItems', N'mcJobMenuItems', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobMenuItems ADD CONSTRAINT
	PK_mcJobMenuItems PRIMARY KEY CLUSTERED 
	(
	MenuItemRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
Update mcJobMenuItems set KitchenLocRno = (Select KitchenLocRno From KitchenLocations Where Name = 'Bakery') where ColdFlg = 1
Update mcJobMenuItems set KitchenLocRno = (Select KitchenLocRno From KitchenLocations Where Name = 'Hot Kitchen') where HotFlg = 1
GO
COMMIT
