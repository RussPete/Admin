/*
   Sunday, November 24, 201310:32:34 AM
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
EXECUTE sp_rename N'dbo.mcJobMenuItems.ServingPrice', N'Tmp_ServingQuote', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.mcJobMenuItems.ServingCost', N'Tmp_ServingPrice_1', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.mcJobMenuItems.Tmp_ServingQuote', N'ServingQuote', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.mcJobMenuItems.Tmp_ServingPrice_1', N'ServingPrice', 'COLUMN' 
GO
ALTER TABLE dbo.mcJobMenuItems
	DROP COLUMN NonPurchaseFlg
GO
ALTER TABLE dbo.mcJobMenuItems SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
