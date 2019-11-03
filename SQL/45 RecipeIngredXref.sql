/*
   Saturday, September 28, 20139:54:54 PM
   User: 
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
CREATE TABLE dbo.Tmp_RecipeIngredXref
	(
	RecipeIngredRno int NOT NULL IDENTITY (1, 1),
	RecipeRno int NOT NULL,
	RecipeSeq int NOT NULL,
	IngredRno int NULL,
	SubrecipeRno int NULL,
	UnitQty decimal(18, 4) NULL,
	UnitRno int NULL,
	BaseCostPrice decimal(18, 4) NULL,
	Title varchar(50) NULL,
	Note varchar(2000) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_RecipeIngredXref SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_RecipeIngredXref ON
GO
IF EXISTS(SELECT * FROM dbo.RecipeIngredXref)
	 EXEC('INSERT INTO dbo.Tmp_RecipeIngredXref (RecipeIngredRno, RecipeRno, RecipeSeq, IngredRno, SubrecipeRno, UnitQty, UnitRno, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT RecipeIngredRno, RecipeRno, RecipeSeq, IngredRno, SubrecipeRno, UnitQty, UnitRno, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.RecipeIngredXref WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_RecipeIngredXref OFF
GO
DROP TABLE dbo.RecipeIngredXref
GO
EXECUTE sp_rename N'dbo.Tmp_RecipeIngredXref', N'RecipeIngredXref', 'OBJECT' 
GO
ALTER TABLE dbo.RecipeIngredXref ADD CONSTRAINT
	PK_RecipeIngredXref PRIMARY KEY CLUSTERED 
	(
	RecipeIngredRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
