-- add ExportDtTm and ExportId for QuickBooks integration

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
CREATE TABLE dbo.Tmp_Payments
	(
	PaymentRno int NOT NULL IDENTITY (1, 1),
	JobRno int NOT NULL,
	Seq int NOT NULL,
	Amount money NULL,
	PaymentDt datetime NULL,
	Reference varchar(130) NULL,
	Method varchar(12) NULL,
	Type varchar(12) NULL,
	PurchaseOrder varchar(50) NULL,
	CCReqAmt money NULL,
	CCType varchar(20) NULL,
	CCNum varchar(100) NULL,
	CCExpDt varchar(6) NULL,
	CCSecCode varchar(50) NULL,
	CCName varchar(50) NULL,
	CCAddr varchar(50) NULL,
	CCZip varchar(15) NULL,
	CCStatus varchar(30) NULL,
	CCResult varchar(150) NULL,
	CCAuth varchar(10) NULL,
	CCReference varchar(130) NULL,
	CCPmtDtTm datetime NULL,
	CCPmtUser varchar(20) NULL,
	CCPrintDtTm datetime NULL,
	CCPrintUser varchar(20) NULL,
	ExportDtTm datetime NULL,
	ExportId varchar(36) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Payments SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Payments ON
GO
IF EXISTS(SELECT * FROM dbo.Payments)
	 EXEC('INSERT INTO dbo.Tmp_Payments (PaymentRno, JobRno, Seq, Amount, PaymentDt, Reference, Method, Type, PurchaseOrder, CCReqAmt, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCAddr, CCZip, CCStatus, CCResult, CCAuth, CCReference, CCPmtDtTm, CCPmtUser, CCPrintDtTm, CCPrintUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT PaymentRno, JobRno, Seq, Amount, PaymentDt, Reference, Method, Type, PurchaseOrder, CCReqAmt, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCAddr, CCZip, CCStatus, CCResult, CCAuth, CCReference, CCPmtDtTm, CCPmtUser, CCPrintDtTm, CCPrintUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Payments WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Payments OFF
GO
DROP TABLE dbo.Payments
GO
EXECUTE sp_rename N'dbo.Tmp_Payments', N'Payments', 'OBJECT' 
GO
ALTER TABLE dbo.Payments ADD CONSTRAINT
	PK_Payments PRIMARY KEY CLUSTERED 
	(
	PaymentRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT