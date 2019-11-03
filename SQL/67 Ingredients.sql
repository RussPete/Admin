-- add StockedPurchaseQty

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
CREATE TABLE dbo.Tmp_Ingredients
	(
	IngredRno int NOT NULL IDENTITY (1, 1),
	Name varchar(50) NULL,
	DryLiquid char(1) NULL,
	StockedFlg bit NULL,
	StockedPurchaseQty decimal(18, 4) NULL,
	PrefBrands varchar(1000) NULL,
	RejBrands varchar(1000) NULL,
	PrefVendors varchar(100) NULL,
	RejVendors varchar(100) NULL,
	IntNote varchar(2000) NULL,
	CT_ItemID int NULL,
	NonPurchaseFlg bit NULL,
	MenuItemAsIsFlg bit NULL,
	HideFlg bit NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Ingredients SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Ingredients ON
GO
IF EXISTS(SELECT * FROM dbo.Ingredients)
	 EXEC('INSERT INTO dbo.Tmp_Ingredients (IngredRno, Name, DryLiquid, StockedFlg, PrefBrands, RejBrands, PrefVendors, RejVendors, IntNote, CT_ItemID, NonPurchaseFlg, MenuItemAsIsFlg, HideFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT IngredRno, Name, DryLiquid, StockedFlg, PrefBrands, RejBrands, PrefVendors, RejVendors, IntNote, CT_ItemID, NonPurchaseFlg, MenuItemAsIsFlg, HideFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Ingredients WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Ingredients OFF
GO
DROP TABLE dbo.Ingredients
GO
EXECUTE sp_rename N'dbo.Tmp_Ingredients', N'Ingredients', 'OBJECT' 
GO
ALTER TABLE dbo.Ingredients ADD CONSTRAINT
	PK_Ingredients PRIMARY KEY CLUSTERED 
	(
	IngredRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
