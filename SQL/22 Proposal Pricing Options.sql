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
CREATE TABLE dbo.Tmp_ProposalPricingOptions
	(
	PropPricingRno int NOT NULL IDENTITY (1, 1),
	PricingCode varchar(20) NULL,
	Seq int NULL,
	OptionType varchar(10) NULL,
	OptionCheckboxDesc varchar(50) NULL,
	OptionPropDesc varchar(50) NULL,
	OptionDefault varchar(50) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL,
	DeletedDtTm datetime NULL,
	DeletedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_ProposalPricingOptions SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_ProposalPricingOptions ON
GO
IF EXISTS(SELECT * FROM dbo.ProposalPricingOptions)
	 EXEC('INSERT INTO dbo.Tmp_ProposalPricingOptions (PropPricingRno, PricingCode, Seq, OptionType, OptionCheckboxDesc, OptionPropDesc, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, DeletedDtTm, DeletedUser)
		SELECT PropPricingRno, PricingCode, Seq, OptionType, OptionCheckboxDesc, OptionPropDesc, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, DeletedDtTm, DeletedUser FROM dbo.ProposalPricingOptions WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_ProposalPricingOptions OFF
GO
DROP TABLE dbo.ProposalPricingOptions
GO
EXECUTE sp_rename N'dbo.Tmp_ProposalPricingOptions', N'ProposalPricingOptions', 'OBJECT' 
GO
COMMIT


Update ProposalPricingOptions Set OptionDefault = '1' Where PricingCode = 'Service' And OptionCheckboxDesc = 'Buffet setup'
Update ProposalPricingOptions Set OptionDefault = '1' Where PricingCode = 'Service' And OptionCheckboxDesc = 'Buffet service'
Update ProposalPricingOptions Set OptionDefault = '1' Where PricingCode = 'Service' And OptionCheckboxDesc = 'General clean-up'

