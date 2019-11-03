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
CREATE TABLE dbo.Tmp_mcJobServices
	(
	ServiceRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	ServiceSeq int NOT NULL,
	ServiceItem varchar(50) NULL,
	MCCRespFlg tinyint NULL,
	CustomerRespFlg tinyint NULL,
	Note varchar(128) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobServices SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobServices OFF
GO
IF EXISTS(SELECT * FROM dbo.mcJobServices)
	 EXEC('INSERT INTO dbo.Tmp_mcJobServices (JobRno, ServiceSeq, ServiceItem, MCCRespFlg, CustomerRespFlg, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT JobRno, ServiceSeq, ServiceItem, MCCRespFlg, CustomerRespFlg, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobServices WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.mcJobServices
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobServices', N'mcJobServices', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobServices ADD CONSTRAINT
	PK_JobServices PRIMARY KEY CLUSTERED 
	(
	ServiceRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_JobServices_Jobs ON dbo.mcJobServices
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
