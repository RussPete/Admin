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
	MultItems varchar(4096) NULL,
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
	 EXEC('INSERT INTO dbo.Tmp_mcJobMenuItems (MenuItemRno, Category, MenuItem, KitchenLocRno, HotFlg, ColdFlg, HideFlg, MultSelFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT MenuItemRno, Category, MenuItem, KitchenLocRno, HotFlg, ColdFlg, HideFlg, MultSelFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobMenuItems WITH (HOLDLOCK TABLOCKX)')
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

drop table MenuItemXref
go 