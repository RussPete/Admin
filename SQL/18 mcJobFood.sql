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
CREATE TABLE dbo.Tmp_mcJobFood
	(
	JobFoodRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	FoodSeq int NULL,
	Category varchar(50) NULL,
	MenuItem varchar(50) NULL,
	MenuItemRno int NULL,
	Qty int NULL,
	QtyNote varchar(128) NULL,
	ServiceNote varchar(128) NULL,
	ProposalSeq int NULL,
	ProposalMenuItem varchar(100) NULL,
	ProposalHideFlg bit NOT NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobFood SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_mcJobFood ADD CONSTRAINT
	DF_mcJobFood_ProposalHideFlg DEFAULT 0 FOR ProposalHideFlg
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobFood ON
GO
IF EXISTS(SELECT * FROM dbo.mcJobFood)
	 EXEC('INSERT INTO dbo.Tmp_mcJobFood (JobFoodRno, JobRno, FoodSeq, Category, MenuItem, MenuItemRno, Qty, QtyNote, ServiceNote, ProposalSeq, ProposalMenuItem, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT JobFoodRno, JobRno, FoodSeq, Category, MenuItem, MenuItemRno, Qty, QtyNote, ServiceNote, ProposalSeq, ProposalMenuItem, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobFood WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobFood OFF
GO
DROP TABLE dbo.mcJobFood
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobFood', N'mcJobFood', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobFood ADD CONSTRAINT
	PK_JobFood PRIMARY KEY CLUSTERED 
	(
	JobFoodRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX Cat_mcJobFood ON dbo.mcJobFood
	(
	Category
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX Item_mcJobFood ON dbo.mcJobFood
	(
	MenuItem
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
