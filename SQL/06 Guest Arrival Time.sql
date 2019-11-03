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
CREATE TABLE dbo.Tmp_mcJobs
	(
	JobRno int NOT NULL,
	Customer varchar(50) NULL,
	TaxExemptFlg tinyint NULL,
	ContactName varchar(50) NULL,
	ContactPhone varchar(20) NULL,
	ContactCell varchar(20) NULL,
	ContactFax varchar(20) NULL,
	ContactEmail varchar(80) NULL,
	EventType varchar(50) NULL,
	ServiceType varchar(50) NULL,
	Location varchar(80) NULL,
	LocationDirections varchar(1024) NULL,
	Vehicle varchar(50) NULL,
	Carts varchar(50) NULL,
	PrintDirectionsFlg tinyint NULL,
	JobDate datetime NULL,
	DepartureTime datetime NULL,
	ArrivalTime datetime NULL,
	GuestArrivalTime datetime NULL,
	MealTime datetime NULL,
	EndTime datetime NULL,
	NumMenServing int NULL,
	NumWomenServing int NULL,
	NumChildServing int NULL,
	PricePerPerson money NULL,
	PmtMethod varchar(20) NULL,
	JobNotes varchar(4096) NULL,
	Status varchar(20) NULL,
	InvDate datetime NULL,
	InvEventCount int NULL,
	InvPricePerPerson money NULL,
	SubTotTaxPct decimal(18, 4) NULL,
	ServiceSubTotPctFlg bit NULL,
	ServiceSubTotPct decimal(18, 4) NULL,
	ServiceAmt money NULL,
	ServiceTaxPct decimal(18, 4) NULL,
	DeliverySubTotPctFlg bit NULL,
	DeliverySubTotPct decimal(18, 4) NULL,
	DeliveryAmt money NULL,
	DeliveryTaxPct decimal(18, 4) NULL,
	FuelTravelSubTotPctFlg bit NULL,
	FuelTravelSubTotPct decimal(18, 4) NULL,
	FuelTravelAmt money NULL,
	FuelTravelTaxPct decimal(18, 4) NULL,
	FacilitySubTotPctFlg bit NULL,
	FacilitySubTotPct decimal(18, 4) NULL,
	FacilityAmt money NULL,
	FacilityTaxPct decimal(18, 4) NULL,
	GratuitySubTotPctFlg bit NULL,
	GratuitySubTotPct decimal(18, 4) NULL,
	GratuityAmt money NULL,
	GratuityTaxPct decimal(18, 4) NULL,
	RentalsDesc varchar(100) NULL,
	RentalsSubTotPctFlg bit NULL,
	RentalsSubTotPct decimal(18, 4) NULL,
	RentalsAmt money NULL,
	RentalsTaxPct decimal(18, 4) NULL,
	Adj1Desc varchar(50) NULL,
	Adj1SubTotPctFlg bit NULL,
	Adj1SubTotPct decimal(18, 4) NULL,
	Adj1Amt money NULL,
	Adj1TaxPct decimal(18, 4) NULL,
	Adj2Desc varchar(50) NULL,
	Adj2SubTotPctFlg bit NULL,
	Adj2SubTotPct decimal(18, 4) NULL,
	Adj2Amt money NULL,
	Adj2TaxPct decimal(18, 4) NULL,
	InvoiceMsg varchar(200) NULL,
	InvDeliveryMethod varchar(20) NULL,
	InvDeliveryDate datetime NULL,
	InvEmail varchar(80) NULL,
	InvFax varchar(30) NULL,
	DepAmt money NULL,
	DepDtTm datetime NULL,
	DepRef varchar(30) NULL,
	PaymentMsg varchar(200) NULL,
	InvPmtDate datetime NULL,
	InvPmtMethod varchar(20) NULL,
	CheckRef varchar(50) NULL,
	CCType varchar(20) NULL,
	CCNum varchar(30) NULL,
	CCExpDt varchar(6) NULL,
	CCSecCode varchar(6) NULL,
	CCName varchar(50) NULL,
	CCStreet varchar(50) NULL,
	CCZip varchar(15) NULL,
	CCBatchID varchar(10) NULL,
	CCInvNum varchar(10) NULL,
	CCApprCode varchar(10) NULL,
	CCAVSResp varchar(10) NULL,
	CCFailedFlg tinyint NULL,
	CCFailedMsg varchar(50) NULL,
	CCSettleDtTm datetime NULL,
	MCCLinens varchar(200) NULL,
	DiamondLinens varchar(200) NULL,
	SusanLinens varchar(200) NULL,
	CUELinens varchar(200) NULL,
	Shirts varchar(50) NULL,
	Aprons varchar(50) NULL,
	FinalDtTm datetime NULL,
	FinalUser varchar(20) NULL,
	ProcessedDtTm datetime NULL,
	ProcessedUser varchar(20) NULL,
	EditProcessedDtTm datetime NULL,
	EditProcessedUser varchar(20) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL,
	InvCreatedDtTm datetime NULL,
	InvCreatedUser varchar(20) NULL,
	InvUpdatedDtTm datetime NULL,
	InvUpdatedUser varchar(20) NULL,
	CompletedDtTm datetime NULL,
	CompletedUser varchar(20) NULL,
	ConfirmedDtTm datetime NULL,
	ConfirmedUser varchar(20) NULL,
	PrintedDtTm datetime NULL,
	PrintedUser varchar(20) NULL,
	InvoicedDtTm datetime NULL,
	InvoicedUser varchar(20) NULL,
	CancelledDtTm datetime NULL,
	CancelledUser varchar(20) NULL,
	InvEmailedDtTm datetime NULL,
	InvEmailedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobs SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.mcJobs)
	 EXEC('INSERT INTO dbo.Tmp_mcJobs (JobRno, Customer, TaxExemptFlg, ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail, EventType, ServiceType, Location, LocationDirections, Vehicle, Carts, PrintDirectionsFlg, JobDate, DepartureTime, ArrivalTime, MealTime, EndTime, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, JobNotes, Status, InvDate, InvEventCount, InvPricePerPerson, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepAmt, DepDtTm, DepRef, PaymentMsg, InvPmtDate, InvPmtMethod, CheckRef, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCStreet, CCZip, CCBatchID, CCInvNum, CCApprCode, CCAVSResp, CCFailedFlg, CCFailedMsg, CCSettleDtTm, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser)
		SELECT JobRno, Customer, TaxExemptFlg, ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail, EventType, ServiceType, Location, LocationDirections, Vehicle, Carts, PrintDirectionsFlg, JobDate, DepartureTime, ArrivalTime, MealTime, EndTime, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, JobNotes, Status, InvDate, InvEventCount, InvPricePerPerson, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepAmt, DepDtTm, DepRef, PaymentMsg, InvPmtDate, InvPmtMethod, CheckRef, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCStreet, CCZip, CCBatchID, CCInvNum, CCApprCode, CCAVSResp, CCFailedFlg, CCFailedMsg, CCSettleDtTm, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser FROM dbo.mcJobs WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.mcJobs
GO
EXECUTE sp_rename N'dbo.Tmp_mcJobs', N'mcJobs', 'OBJECT' 
GO
ALTER TABLE dbo.mcJobs ADD CONSTRAINT
	PK_Jobs PRIMARY KEY CLUSTERED 
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX Cust_mcJobs ON dbo.mcJobs
	(
	Customer
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX Date_mcJobs ON dbo.mcJobs
	(
	JobDate
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
