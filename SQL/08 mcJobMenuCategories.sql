/* add created and updated fields to mcJobMenuCategories */
/*
   Monday, January 21, 20135:16:48 PM
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
ALTER TABLE dbo.mcJobMenuCategories ADD
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
GO
ALTER TABLE dbo.mcJobMenuCategories SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
