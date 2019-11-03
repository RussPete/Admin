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
CREATE TABLE dbo.Tmp_mcJobSupplies
	(
	SupplyRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	SupplySeq int NOT NULL,
	SupplyItem varchar(50) NULL,
	Qty int NULL,
	Note varchar(128) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobSupplies SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobSupplies OFF
GO
IF EXISTS(SELECT * FROM dbo.mcJobSupplies)
	 EXEC('INSERT INTO dbo.Tmp_mcJobSupplies (JobRno, SupplySeq, SupplyItem, Qty, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT JobRno, SupplySeq, SupplyItem, Qty, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobSupplies WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.mcJobSupplies
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobSupplies', N'mcJobSupplies', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobSupplies ADD CONSTRAINT
	PK_JobSupplies PRIMARY KEY CLUSTERED 
	(
	SupplyRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_JobSupplies_Jobs ON dbo.mcJobSupplies
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
