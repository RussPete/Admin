-- drop old payment and customer/contact fields, add contact index

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
DROP INDEX Cust_mcJobs ON dbo.mcJobs
GO
CREATE NONCLUSTERED INDEX Contact_mcJobs ON dbo.mcJobs
	(
	ContactRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.mcJobs
	DROP COLUMN Customer, TaxExemptFlg, ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail, CustomerExportId, PmtAmt1, PmtDt1, PmtRef1, PmtMethod1, PmtType1, CCReqAmt1, CCType1, CCNum1, CCExpDt1, CCSecCode1, CCName1, CCAddr1, CCZip1, CCStatus1, CCResult1, CCAuth1, CCRef1, CCPmtDtTm1, CCPmtUser1, CCPrintDtTm1, CCPrintUser1, PmtAmt2, PmtDt2, PmtRef2, PmtMethod2, PmtType2, CCReqAmt2, CCType2, CCNum2, CCExpDt2, CCSecCode2, CCName2, CCAddr2, CCZip2, CCStatus2, CCResult2, CCAuth2, CCRef2, CCPmtDtTm2, CCPmtUser2, CCPrintDtTm2, CCPrintUser2, PmtAmt3, PmtDt3, PmtRef3, PmtMethod3, PmtType3, CCReqAmt3, CCType3, CCNum3, CCExpDt3, CCSecCode3, CCName3, CCAddr3, CCZip3, CCStatus3, CCResult3, CCAuth3, CCRef3, CCPmtDtTm3, CCPmtUser3, CCPrintDtTm3, CCPrintUser3
GO
ALTER TABLE dbo.mcJobs SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
