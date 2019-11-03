/* Add several fields: NumServings, Yields, Portioins, Cost, etc

   Sunday, July 14, 20136:24:01 PM
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
CREATE TABLE dbo.Tmp_Recipes
	(
	RecipeRno int NOT NULL IDENTITY (1, 1),
	Name varchar(50) NULL,
	NumServings decimal(18, 4) NULL,
	MenServingRatio decimal(18, 4) NULL,
	WomenServingRatio decimal(18, 4) NULL,
	ChildServingRatio decimal(18, 4) NULL,
	YieldQty decimal(18, 4) NULL,
	YieldUnitRno int NULL,
	PortionQty decimal(18, 4) NULL,
	PortionUnitRno int NULL,
	CostBasePct decimal(18, 4) NULL,
	Instructions varchar(8000) NULL,
	Source varchar(1000) NULL,
	Images varchar(4000) NULL,
	IntNote varchar(2000) NULL,
	CT_RecipeID int NULL,
	ActiveFlg int NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Recipes SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Recipes ON
GO
IF EXISTS(SELECT * FROM dbo.Recipes)
	 EXEC('INSERT INTO dbo.Tmp_Recipes (RecipeRno, IntNote, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT RecipeRno, IntNote, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Recipes WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Recipes OFF
GO
DROP TABLE dbo.Recipes
GO
EXECUTE sp_rename N'dbo.Tmp_Recipes', N'Recipes', 'OBJECT' 
GO
ALTER TABLE dbo.Recipes ADD CONSTRAINT
	PK_Recipes PRIMARY KEY CLUSTERED 
	(
	RecipeRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
