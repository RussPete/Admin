-- Add MenuType
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
	JobDesc varchar(80) NULL,
	ContactRno int NULL,
	ContractSendDtTm datetime NULL,
	ContractSignedFileName varchar(200) NULL,
	ResponsibleParty varchar(50) NULL,
	BookedBy varchar(50) NULL,
	ConfirmedBy varchar(50) NULL,
	JobType varchar(20) NULL,
	PmtTerms varchar(20) NULL,
	ProposalFlg bit NOT NULL,
	EventType varchar(50) NULL,
	EventTypeDetails varchar(200) NULL,
	ServiceType varchar(50) NULL,
	Location varchar(80) NULL,
	LocationDirections varchar(1024) NULL,
	PrintDirectionsFlg tinyint NULL,
	Vehicle varchar(50) NULL,
	Carts varchar(50) NULL,
	NumBuffets int NULL,
	BuffetSpace varchar(200) NULL,
	JobDate datetime NULL,
	LoadTime datetime NULL,
	DepartureTime datetime NULL,
	ArrivalTime datetime NULL,
	GuestArrivalTime datetime NULL,
	MealTime datetime NULL,
	EndTime datetime NULL,
	CeremonyTime datetime NULL,
	CeremonyFlg bit NULL,
	CeremonyLocation varchar(80) NULL,
	NumMenServing int NULL,
	NumWomenServing int NULL,
	NumChildServing int NULL,
	PricePerPerson money NULL,
	PmtMethod varchar(20) NULL,
	MenuType varchar(30) NULL,
	PONumber varchar(50) NULL,
	Demographics varchar(1024) NULL,
	JobNotes varchar(4096) NULL,
	ProductionNotes varchar(4096) NULL,
	Crew varchar(200) NULL,
	Status varchar(20) NULL,
	ConfirmDeadlineDate datetime NULL,
	InvDate datetime NULL,
	InvEventCount int NULL,
	InvPricePerPerson money NULL,
	SubTotAmt money NULL,
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
	VoluntaryGratuitySubTotPctFlg bit NULL,
	VoluntaryGratuitySubTotPct decimal(18, 4) NULL,
	VoluntaryGratuityAmt money NULL,
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
	DepositAmt money NULL,
	CCPmtFeeFlg bit NULL,
	CCFeePct decimal(18, 4) NULL,
	CCFeeAmt money NULL,
	CCFeeTaxPct decimal(18, 4) NULL,
	PreTaxSubTotAmt money NULL,
	SalesTaxTotAmt money NULL,
	InvTotAmt money NULL,
	InvBalAmt money NULL,
	MCCLinens varchar(200) NULL,
	DiamondLinens varchar(200) NULL,
	SusanLinens varchar(200) NULL,
	CUELinens varchar(200) NULL,
	Shirts varchar(50) NULL,
	Aprons varchar(50) NULL,
	DisposableFlg bit NULL,
	SlingShiftId bigint NULL,
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
	FinishConfirmDtTm datetime NULL,
	ConfirmedDtTm datetime NULL,
	ConfirmedUser varchar(20) NULL,
	PrintedDtTm datetime NULL,
	PrintedUser varchar(20) NULL,
	ExportDtTm datetime NULL,
	ExportId varchar(36) NULL,
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
	ConfirmEmailedUser varchar(20) NULL,
	FinalConfirmEmailedDtTm datetime NULL,
	FinalConfirmEmailedUser varchar(20) NULL,
	DepositInvPrintedDtTm datetime NULL,
	DepositInvPrintedUser varchar(20) NULL,
	DepositInvEmailedDtTm datetime NULL,
	DepositInvEmailedUser varchar(20) NULL,
	PaidInFullDtTm datetime NULL,
	PaidInFullUser varchar(20) NULL,
	Guid uniqueidentifier NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_mcJobs SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_mcJobs ADD CONSTRAINT
	DF_mcJobs_ProposalFlg DEFAULT ((0)) FOR ProposalFlg
GO
IF EXISTS(SELECT * FROM dbo.mcJobs)
	 EXEC('INSERT INTO dbo.Tmp_mcJobs (JobRno, JobDesc, ContactRno, ContractSendDtTm, ContractSignedFileName, ResponsibleParty, BookedBy, ConfirmedBy, JobType, PmtTerms, ProposalFlg, EventType, EventTypeDetails, ServiceType, Location, LocationDirections, PrintDirectionsFlg, Vehicle, Carts, NumBuffets, BuffetSpace, JobDate, LoadTime, DepartureTime, ArrivalTime, GuestArrivalTime, MealTime, EndTime, CeremonyTime, CeremonyFlg, CeremonyLocation, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, PONumber, Demographics, JobNotes, ProductionNotes, Crew, Status, ConfirmDeadlineDate, InvDate, InvEventCount, InvPricePerPerson, SubTotAmt, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, ServicePropDesc, ServicePropDescCustFlg, ServicePropOptions, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, DeliveryPropDesc, DeliveryPropDescCustFlg, DeliveryPropOptions, ChinaDesc, ChinaSubTotPctFlg, ChinaSubTotPct, ChinaAmt, ChinaTaxPct, ChinaPropDesc, ChinaPropDescCustFlg, ChinaPropOptions, AddServiceSubTotPctFlg, AddServiceSubTotPct, AddServiceAmt, AddServiceTaxPct, AddServicePropDesc, AddServicePropDescCustFlg, AddServicePropOptions, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FuelTravelPropDesc, FuelTravelPropDescCustFlg, FuelTravelPropOptions, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, FacilityPropDesc, FacilityPropDescCustFlg, FacilityPropOptions, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, VoluntaryGratuitySubTotPctFlg, VoluntaryGratuitySubTotPct, VoluntaryGratuityAmt, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, RentalsPropDesc, RentalsPropDescCustFlg, RentalsPropOptions, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj1PropDesc, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, Adj2PropDesc, EstTotPropDesc, EstTotPropDescCustFlg, EstTotPropOptions, DepositPropDesc, DepositPropDescCustFlg, DepositPropOptions, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepositAmt, CCPmtFeeFlg, CCFeePct, CCFeeAmt, CCFeeTaxPct, PreTaxSubTotAmt, SalesTaxTotAmt, InvTotAmt, InvBalAmt, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, DisposableFlg, SlingShiftId, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, FinishConfirmDtTm, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, ExportDtTm, ExportId, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser, PropCreatedDtTm, PropCreatedUser, PropUpdatedDtTm, PropUpdatedUser, PropGeneratedDtTm, PropGeneratedUser, PropEmailedDtTm, PropEmailedUser, ConfirmEmailedDtTm, ConfirmEmailedUser, FinalConfirmEmailedDtTm, FinalConfirmEmailedUser, DepositInvPrintedDtTm, DepositInvPrintedUser, DepositInvEmailedDtTm, DepositInvEmailedUser, PaidInFullDtTm, PaidInFullUser, Guid)
		SELECT JobRno, JobDesc, ContactRno, ContractSendDtTm, ContractSignedFileName, ResponsibleParty, BookedBy, ConfirmedBy, JobType, PmtTerms, ProposalFlg, EventType, EventTypeDetails, ServiceType, Location, LocationDirections, PrintDirectionsFlg, Vehicle, Carts, NumBuffets, BuffetSpace, JobDate, LoadTime, DepartureTime, ArrivalTime, GuestArrivalTime, MealTime, EndTime, CeremonyTime, CeremonyFlg, CeremonyLocation, NumMenServing, NumWomenServing, NumChildServing, PricePerPerson, PmtMethod, PONumber, Demographics, JobNotes, ProductionNotes, Crew, Status, ConfirmDeadlineDate, InvDate, InvEventCount, InvPricePerPerson, SubTotAmt, SubTotTaxPct, ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, ServiceTaxPct, ServicePropDesc, ServicePropDescCustFlg, ServicePropOptions, DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, DeliveryTaxPct, DeliveryPropDesc, DeliveryPropDescCustFlg, DeliveryPropOptions, ChinaDesc, ChinaSubTotPctFlg, ChinaSubTotPct, ChinaAmt, ChinaTaxPct, ChinaPropDesc, ChinaPropDescCustFlg, ChinaPropOptions, AddServiceSubTotPctFlg, AddServiceSubTotPct, AddServiceAmt, AddServiceTaxPct, AddServicePropDesc, AddServicePropDescCustFlg, AddServicePropOptions, FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, FuelTravelTaxPct, FuelTravelPropDesc, FuelTravelPropDescCustFlg, FuelTravelPropOptions, FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, FacilityTaxPct, FacilityPropDesc, FacilityPropDescCustFlg, FacilityPropOptions, GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, GratuityTaxPct, VoluntaryGratuitySubTotPctFlg, VoluntaryGratuitySubTotPct, VoluntaryGratuityAmt, RentalsDesc, RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt, RentalsTaxPct, RentalsPropDesc, RentalsPropDescCustFlg, RentalsPropOptions, Adj1Desc, Adj1SubTotPctFlg, Adj1SubTotPct, Adj1Amt, Adj1TaxPct, Adj1PropDesc, Adj2Desc, Adj2SubTotPctFlg, Adj2SubTotPct, Adj2Amt, Adj2TaxPct, Adj2PropDesc, EstTotPropDesc, EstTotPropDescCustFlg, EstTotPropOptions, DepositPropDesc, DepositPropDescCustFlg, DepositPropOptions, InvoiceMsg, InvDeliveryMethod, InvDeliveryDate, InvEmail, InvFax, DepositAmt, CCPmtFeeFlg, CCFeePct, CCFeeAmt, CCFeeTaxPct, PreTaxSubTotAmt, SalesTaxTotAmt, InvTotAmt, InvBalAmt, MCCLinens, DiamondLinens, SusanLinens, CUELinens, Shirts, Aprons, DisposableFlg, SlingShiftId, FinalDtTm, FinalUser, ProcessedDtTm, ProcessedUser, EditProcessedDtTm, EditProcessedUser, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser, InvCreatedDtTm, InvCreatedUser, InvUpdatedDtTm, InvUpdatedUser, CompletedDtTm, CompletedUser, FinishConfirmDtTm, ConfirmedDtTm, ConfirmedUser, PrintedDtTm, PrintedUser, ExportDtTm, ExportId, InvoicedDtTm, InvoicedUser, CancelledDtTm, CancelledUser, InvEmailedDtTm, InvEmailedUser, PropCreatedDtTm, PropCreatedUser, PropUpdatedDtTm, PropUpdatedUser, PropGeneratedDtTm, PropGeneratedUser, PropEmailedDtTm, PropEmailedUser, ConfirmEmailedDtTm, ConfirmEmailedUser, FinalConfirmEmailedDtTm, FinalConfirmEmailedUser, DepositInvPrintedDtTm, DepositInvPrintedUser, DepositInvEmailedDtTm, DepositInvEmailedUser, PaidInFullDtTm, PaidInFullUser, Guid FROM dbo.mcJobs WITH (HOLDLOCK TABLOCKX)')
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
CREATE NONCLUSTERED INDEX Contact_mcJobs ON dbo.mcJobs
	(
	ContactRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX Date_mcJobs ON dbo.mcJobs
	(
	JobDate
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX Guid_mcJobs ON dbo.mcJobs
	(
	JobRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
