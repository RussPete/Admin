-- Add Color & Ordered

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
CREATE TABLE dbo.Tmp_JobLinens
	(
	LinenRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	Seq int NOT NULL,
	Description varchar(20) NOT NULL,
	Caterer varchar(60) NULL,
	Color varchar(20) NULL,
	Ordered varchar(20) NULL,
	Other varchar(60) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_JobLinens SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_JobLinens ON
GO
IF EXISTS(SELECT * FROM dbo.JobLinens)
	 EXEC('INSERT INTO dbo.Tmp_JobLinens (LinenRno, JobRno, Seq, Description, Caterer, Other, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT LinenRno, JobRno, Seq, Description, Caterer, Other, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.JobLinens WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_JobLinens OFF
GO
DROP TABLE dbo.JobLinens
GO
EXECUTE sp_rename N'dbo.Tmp_JobLinens', N'JobLinens', 'OBJECT' 
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
COMMIT
