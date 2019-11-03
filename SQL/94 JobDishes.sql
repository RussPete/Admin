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
CREATE TABLE dbo.Tmp_mcJobDishes
	(
	DishRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	DishSeq int NOT NULL,
	DishItem varchar(50) NULL,
	Qty int NULL,
	Note varchar(128) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobDishes SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobDishes OFF
GO
IF EXISTS(SELECT * FROM dbo.mcJobDishes)
	 EXEC('INSERT INTO dbo.Tmp_mcJobDishes (JobRno, DishSeq, DishItem, Qty, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT JobRno, DishSeq, DishItem, Qty, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobDishes WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.mcJobDishes
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobDishes', N'mcJobDishes', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobDishes ADD CONSTRAINT
	PK_mcJobDishes PRIMARY KEY CLUSTERED 
	(
	DishRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_JobDishes_Jobs ON dbo.mcJobDishes
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT

