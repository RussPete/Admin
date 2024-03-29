/* add PropPerPersonDesc, PropPerPersonDescCustFlg, PropPerItemDesc, PropPerItemDescCustFlg, 
	PropAllInclDesc. PropAllInclDescCustFlg bit NULL,
	to JobInvoicePrices 
   add data to ProposalPricingOptions for PerPerson prices
*/
/*
   Monday, January 21, 20132:31:54 PM
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
CREATE TABLE dbo.Tmp_JobInvoicePrices
	(
	Rno int NULL,
	JobRno int NULL,
	Seq int NULL,
	PriceType varchar(20) NULL,
	InvPerPersonCount int NULL,
	InvPerPersonDesc varchar(50) NULL,
	InvPerPersonPrice money NULL,
	InvPerItemCount int NULL,
	InvPerItemDesc varchar(50) NULL,
	InvPerItemPrice money NULL,
	InvAllInclDesc varchar(50) NULL,
	InvAllInclPrice money NULL,
	PropPerPersonDesc varchar(100) NULL,
	PropPerPersonDescCustFlg bit NULL,
	PropPerItemDesc varchar(100) NULL,
	PropPerItemDescCustFlg bit NULL,
	PropAllInclDesc varchar(100) NULL,
	PropAllInclDescCustFlg bit NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_JobInvoicePrices SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.JobInvoicePrices)
	 EXEC('INSERT INTO dbo.Tmp_JobInvoicePrices (Rno, JobRno, Seq, PriceType, InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, InvPerItemCount, InvPerItemDesc, InvPerItemPrice, InvAllInclDesc, InvAllInclPrice, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT Rno, JobRno, Seq, PriceType, InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, InvPerItemCount, InvPerItemDesc, InvPerItemPrice, InvAllInclDesc, InvAllInclPrice, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.JobInvoicePrices WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.JobInvoicePrices
GO
EXECUTE sp_rename N'dbo.Tmp_JobInvoicePrices', N'JobInvoicePrices', 'OBJECT' 
GO
COMMIT
