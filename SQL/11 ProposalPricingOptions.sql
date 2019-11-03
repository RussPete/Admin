/* create ProposalPricingOptions */
/*
   Monday, January 21, 20135:17:25 PM
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
CREATE TABLE dbo.ProposalPricingOptions
	(
	PropPricingRno int NOT NULL,
	PricingCode varchar(20) NULL,
	Seq int NULL,
	OptionCheckboxDesc varchar(50) NULL,
	OptionPropDesc varchar(50) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL,
	DeletedDtTm datetime NULL,
	DeletedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ProposalPricingOptions SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
