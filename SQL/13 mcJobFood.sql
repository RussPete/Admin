/* add MenuItemRno, ProposalSeq, ProposalMenuItem to mcJobFood 
   set default values for new fields */
    
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
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobFood SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_mcJobFood OFF
GO
IF EXISTS(SELECT * FROM dbo.mcJobFood)
	 EXEC('INSERT INTO dbo.Tmp_mcJobFood (JobRno, FoodSeq, Category, MenuItem, Qty, QtyNote, ServiceNote, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT JobRno, FoodSeq, Category, MenuItem, Qty, QtyNote, ServiceNote, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.mcJobFood WITH (HOLDLOCK TABLOCKX)')
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


Begin Transaction

Update mcJobFood Set MenuItemRno = (Select Top 1 MenuItemRno From mcJobMenuItems i Where i.Category = mcJobFood.Category And i.MenuItem = mcJobFood.MenuItem)
Update mcJobFood Set ProposalMenuItem = (Select Top 1 Coalesce(ProposalMenuItem, MenuItem) From mcJobMenuItems Where MenuItemRno = mcJobFood.MenuItemRno)
Update mcJobMenuItems Set ProposalMenuItem = MenuItem Where ProposalMenuItem Is Null Or ProposalMenuItem = ''
Update mcJobFood Set ProposalMenuItem = MenuItem Where ProposalMenuItem Is Null Or ProposalMenuItem = ''

commit