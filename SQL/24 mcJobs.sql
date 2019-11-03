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
ALTER TABLE dbo.mcJobs
	DROP CONSTRAINT DF_mcJobs_ProposalFlg
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
	JobType varchar(20) NULL,
	ProposalFlg bit NOT NULL,
	EventType varchar(50) NULL,
	ServiceType varchar(50) NULL,
	Location varchar(80) NULL,
	LocationDirections varchar(1024) NULL,
	PrintDirectionsFlg tinyint NULL,
	Vehicle varchar(50) NULL,
	Carts varchar(50) NULL,
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
	ServicePropDesc varchar(1000) NULL,
	ServicePropDescCustFlg bit NULL,
	ServicePropOptions varchar(200) NULL,
	DeliverySubTotPctFlg bit NULL,
	DeliverySubTotPct decimal(18, 4) NULL,
	DeliveryAmt money NULL,
	DeliveryTaxPct decimal(18, 4) NULL,
	DeliveryPropDesc varchar(1000) NULL,
	DeliveryPropDescCustFlg bit NULL,
	DeliveryPropOptions varchar(200) NULL,
	ChinaDesc varchar(100) NULL,
	ChinaSubTotPctFlg bit NULL,
	ChinaSubTotPct decimal(18, 4) NULL,
	ChinaAmt money NULL,
	ChinaTaxPct decimal(18, 4) NULL,
	ChinaPropDesc varchar(1000) NULL,
	ChinaPropDescCustFlg bit NULL,
	ChinaPropOptions varchar(200) NULL,
	AddServiceSubTotPctFlg bit NULL,
	AddServiceSubTotPct decimal(18, 4) NULL,
	AddServiceAmt money NULL,
	AddServiceTaxPct decimal(18, 4) NULL,
	AddServicePropDesc varchar(1000) NULL,
	AddServicePropDescCustFlg bit NULL,
	AddServicePropOptions varchar(200) NULL,
	FuelTravelSubTotPctFlg bit NULL,
	FuelTravelSubTotPct decimal(18, 4) NULL,
	FuelTravelAmt money NULL,
	FuelTravelTaxPct decimal(18, 4) NULL,
	FuelTravelPropDesc varchar(1000) NULL,
	FuelTravelPropDescCustFlg bit NULL,
	FuelTravelPropOptions varchar(200) NULL,
	FacilitySubTotPctFlg bit NULL,
	FacilitySubTotPct decimal(18, 4) NULL,
	FacilityAmt money NULL,
	FacilityTaxPct decimal(18, 4) NULL,
	FacilityPropDesc varchar(1000) NULL,
	FacilityPropDescCustFlg bit NULL,
	FacilityPropOptions varchar(200) NULL,
	GratuitySubTotPctFlg bit NULL,
	GratuitySubTotPct decimal(18, 4) NULL,
	GratuityAmt money NULL,
	GratuityTaxPct decimal(18, 4) NULL,
	RentalsDesc varchar(100) NULL,
	RentalsSubTotPctFlg bit NULL,
	RentalsSubTotPct decimal(18, 4) NULL,
	RentalsAmt money NULL,
	RentalsTaxPct decimal(18, 4) NULL,
	RentalsPropDesc varchar(1000) NULL,
	RentalsPropDescCustFlg bit NULL,
	RentalsPropOptions varchar(200) NULL,
	Adj1Desc varchar(50) NULL,
	Adj1SubTotPctFlg bit NULL,
	Adj1SubTotPct decimal(18, 4) NULL,
	Adj1Amt money NULL,
	Adj1TaxPct decimal(18, 4) NULL,
	Adj1PropDesc varchar(1000) NULL,
	Adj2Desc varchar(50) NULL,
	Adj2SubTotPctFlg bit NULL,
	Adj2SubTotPct decimal(18, 4) NULL,
	Adj2Amt money NULL,
	Adj2TaxPct decimal(18, 4) NULL,
	Adj2PropDesc varchar(1000) NULL,
	EstTotPropDesc varchar(1000) NULL,
	EstTotPropDescCustFlg bit NULL,
	EstTotPropOptions varchar(200) NULL,
	DepositPropDesc varchar(1000) NULL,
	DepositPropDescCustFlg bit NULL,
	DepositPropOptions varchar(200) NULL,
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
	InvEmailedUser varchar(20) NULL,
	PropCreatedDtTm datetime NULL,
	PropCreatedUser varchar(20) NULL,
	PropUpdatedDtTm datetime NULL,
	PropUpdatedUser varchar(20) NULL,
	PropGeneratedDtTm datetime NULL,
	PropGeneratedUser varchar(20) NULL,
	PropEmailedDtTm datetime NULL,
	PropEmailedUser varchar(20) NULL,
	ConfirmEmailedDtTm datetime NULL,
	ConfirmEmailedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobs SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_mcJobs ADD CONSTRAINT
	DF_mcJobs_ProposalFlg DEFAULT ((0)) FOR ProposalFlg
GO
IF EXISTS(SELECT * FROM dbo.mcJobs)
	 EXEC('INSERT INTO dbo.Tmp_mcJobs (JobRno, Customer, TaxExemptFlg, ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail, JobType, ProposalFlg, EventType, ServiceType, Location, LocationDirections, PrintDirectionsFlg, Vehicle, Carts, JobDate, DepartureTime, ArrivalTime, GuestArrivalTime, MealTime, EndTime, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, JobNotes, Status, InvDate, InvEventCount, InvPricePerPerson, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, ServicePropDesc, ServicePropDescCustFlg, ServicePropOptions, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, DeliveryPropDesc, DeliveryPropDescCustFlg, DeliveryPropOptions, ChinaDesc, ChinaSubTotPctFlg, ChinaSubTotPct, ChinaAmt, ChinaTaxPct, ChinaPropDesc, ChinaPropDescCustFlg, ChinaPropOptions, AddServiceSubTotPctFlg, AddServiceSubTotPct, AddServiceAmt, AddServiceTaxPct, AddServicePropDesc, AddServicePropDescCustFlg, AddServicePropOptions, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FuelTravelPropDesc, FuelTravelPropDescCustFlg, FuelTravelPropOptions, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, FacilityPropDesc, FacilityPropDescCustFlg, FacilityPropOptions, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, RentalsPropDesc, RentalsPropDescCustFlg, RentalsPropOptions, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj1PropDesc, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, Adj2PropDesc, EstTotPropDesc, EstTotPropDescCustFlg, EstTotPropOptions, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepAmt, DepDtTm, DepRef, PaymentMsg, InvPmtDate, InvPmtMethod, CheckRef, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCStreet, CCZip, CCBatchID, CCInvNum, CCApprCode, CCAVSResp, CCFailedFlg, CCFailedMsg, CCSettleDtTm, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser, PropCreatedDtTm, PropCreatedUser, PropUpdatedDtTm, PropUpdatedUser, PropGeneratedDtTm, PropGeneratedUser, PropEmailedDtTm, PropEmailedUser, ConfirmEmailedDtTm, ConfirmEmailedUser)
		SELECT JobRno, Customer, TaxExemptFlg, ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail, JobType, ProposalFlg, EventType, ServiceType, Location, LocationDirections, PrintDirectionsFlg, Vehicle, Carts, JobDate, DepartureTime, ArrivalTime, GuestArrivalTime, MealTime, EndTime, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, JobNotes, Status, InvDate, InvEventCount, InvPricePerPerson, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, ServicePropDesc, ServicePropDescCustFlg, ServicePropOptions, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, DeliveryPropDesc, DeliveryPropDescCustFlg, DeliveryPropOptions, ChinaDesc, ChinaSubTotPctFlg, ChinaSubTotPct, ChinaAmt, ChinaTaxPct, ChinaPropDesc, ChinaPropDescCustFlg, ChinaPropOptions, AddServiceSubTotPctFlg, AddServiceSubTotPct, AddServiceAmt, AddServiceTaxPct, AddServicePropDesc, AddServicePropDescCustFlg, AddServicePropOptions, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FuelTravelPropDesc, FuelTravelPropDescCustFlg, FuelTravelPropOptions, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, FacilityPropDesc, FacilityPropDescCustFlg, FacilityPropOptions, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, RentalsPropDesc, RentalsPropDescCustFlg, RentalsPropOptions, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj1PropDesc, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, Adj2PropDesc, EstTotPropDesc, EstTotPropDescCustFlg, EstTotPropOptions, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepAmt, DepDtTm, DepRef, PaymentMsg, InvPmtDate, InvPmtMethod, CheckRef, CCType, CCNum, CCExpDt, CCSecCode, CCName, CCStreet, CCZip, CCBatchID, CCInvNum, CCApprCode, CCAVSResp, CCFailedFlg, CCFailedMsg, CCSettleDtTm, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser, PropCreatedDtTm, PropCreatedUser, PropUpdatedDtTm, PropUpdatedUser, PropGeneratedDtTm, PropGeneratedUser, PropEmailedDtTm, PropEmailedUser, ConfirmEmailedDtTm, ConfirmEmailedUser FROM dbo.mcJobs WITH (HOLDLOCK TABLOCKX)')
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
