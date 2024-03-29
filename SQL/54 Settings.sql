/*
   Monday, January 6, 20147:43:16 PM
   User: 
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
CREATE TABLE dbo.Tmp_Settings
	(
	SettingRno int NOT NULL,
	SubTotTaxPct decimal(18, 4) NULL,
	ServiceSubTotPct decimal(18, 4) NULL,
	ServiceTaxPct decimal(18, 4) NULL,
	DeliverySubTotPct decimal(18, 4) NULL,
	DeliveryTaxPct decimal(18, 4) NULL,
	ChinaSubTotPct decimal(18, 4) NULL,
	ChinaTaxPct decimal(18, 4) NULL,
	AddServiceSubTotPct decimal(18, 4) NULL,
	AddServiceTaxPct decimal(18, 4) NULL,
	FuelTravelSubTotPct decimal(18, 4) NULL,
	FuelTravelTaxPct decimal(18, 4) NULL,
	FacilitySubTotPct decimal(18, 4) NULL,
	FacilityTaxPct decimal(18, 4) NULL,
	GratuitySubTotPct decimal(18, 4) NULL,
	GratuityTaxPct decimal(18, 4) NULL,
	RentalsSubTotPct decimal(18, 4) NULL,
	RentalsTaxPct decimal(18, 4) NULL,
	Adj1TaxPct decimal(18, 4) NULL,
	Adj2TaxPct decimal(18, 4) NULL,
	CCFeePct decimal(18, 4) NULL,
	CCFeeTaxPct decimal(18, 4) NULL,
	BaseCostPct decimal(18, 4) NULL,
	AsIsBaseCostPct decimal(18, 4) NULL,
	QuoteDiffPct decimal(18, 4) NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Settings SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Settings)
	 EXEC('INSERT INTO dbo.Tmp_Settings (SettingRno, SubTotTaxPct, ServiceSubTotPct, ServiceTaxPct, DeliverySubTotPct, DeliveryTaxPct, ChinaSubTotPct, ChinaTaxPct, AddServiceSubTotPct, AddServiceTaxPct, FuelTravelSubTotPct, FuelTravelTaxPct, FacilitySubTotPct, FacilityTaxPct, GratuitySubTotPct, GratuityTaxPct, RentalsSubTotPct, RentalsTaxPct, Adj1TaxPct, Adj2TaxPct, CCFeePct, CCFeeTaxPct, BaseCostPct, AsIsBaseCostPct, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT SettingRno, SubTotTaxPct, ServiceSubTotPct, ServiceTaxPct, DeliverySubTotPct, DeliveryTaxPct, ChinaSubTotPct, ChinaTaxPct, AddServiceSubTotPct, AddServiceTaxPct, FuelTravelSubTotPct, FuelTravelTaxPct, FacilitySubTotPct, FacilityTaxPct, GratuitySubTotPct, GratuityTaxPct, RentalsSubTotPct, RentalsTaxPct, Adj1TaxPct, Adj2TaxPct, CCFeePct, CCFeeTaxPct, BaseCostPct, AsIsBaseCostPct, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Settings WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Settings
GO
EXECUTE sp_rename N'dbo.Tmp_Settings', N'Settings', 'OBJECT' 
GO
COMMIT
