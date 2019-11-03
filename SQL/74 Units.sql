-- Add HideFlg
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
CREATE TABLE dbo.Tmp_Units
	(
	UnitRno int NOT NULL IDENTITY (1, 1),
	UnitSingle varchar(20) NULL,
	UnitPlural varchar(20) NULL,
	HideFlg bit NULL,
	CT_UnitID smallint NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Units SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Units ON
GO
IF EXISTS(SELECT * FROM dbo.Units)
	 EXEC('INSERT INTO dbo.Tmp_Units (UnitRno, UnitSingle, UnitPlural, CT_UnitID, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT UnitRno, UnitSingle, UnitPlural, CT_UnitID, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Units WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Units OFF
GO
DROP TABLE dbo.Units
GO
EXECUTE sp_rename N'dbo.Tmp_Units', N'Units', 'OBJECT' 
GO
ALTER TABLE dbo.Units ADD CONSTRAINT
	PK_Units PRIMARY KEY CLUSTERED 
	(
	UnitRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
