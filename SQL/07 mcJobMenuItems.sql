/* add ProposalMenuItem to mcJobMenuItems */

/*
   Monday, January 21, 20132:14:54 PM
   User: sa
   Server: Server
   Database: MarvellousCatering
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_mcJobMenuItems
	(
	MenuItemRno int NOT NULL IDENTITY (1, 1),
	Category varchar(50) NULL,
	MenuItem varchar(50) NULL,
	ProposalMenuItem varchar(100) NULL,
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
	 EXEC('INSERT INTO dbo.Tmp_mcJobMenuItems (MenuItemRno, Category, MenuItem, KitchenLocRno, HotFlg, ColdFlg, HideFlg, MultSelFlg, MultItems, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT MenuItemRno, Category, MenuItem, KitchenLocRno, HotFlg, ColdFlg, HideFlg, MultSelFlg, MultItems, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobMenuItems WITH (HOLDLOCK TABLOCKX)')
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
