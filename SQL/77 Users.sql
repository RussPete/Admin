-- add AccessLevel

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
ALTER TABLE dbo.Users
	DROP CONSTRAINT DF_Users_AdminFlg
GO
ALTER TABLE dbo.Users
	DROP CONSTRAINT DF_Users_DisabledFlg
GO
CREATE TABLE dbo.Tmp_Users
	(
	UserRno bigint NOT NULL IDENTITY (1, 1),
	UserName varchar(20) NOT NULL,
	UserCommonName varchar(50) NULL,
	PasswordSalt varchar(64) NOT NULL,
	Password varchar(64) NOT NULL,
	AccessLevel int NOT NULL,
	AdminFlg bit NOT NULL,
	DisabledFlg bit NOT NULL,
	CreatedDtTm datetime NULL,
	CreatedUser varchar(20) NULL,
	UpdatedDtTm datetime NULL,
	UpdatedUser varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Users SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Users ADD CONSTRAINT
	DF_Users_AccessLevel DEFAULT 0 FOR AccessLevel
GO
ALTER TABLE dbo.Tmp_Users ADD CONSTRAINT
	DF_Users_AdminFlg DEFAULT ((0)) FOR AdminFlg
GO
ALTER TABLE dbo.Tmp_Users ADD CONSTRAINT
	DF_Users_DisabledFlg DEFAULT ((0)) FOR DisabledFlg
GO
SET IDENTITY_INSERT dbo.Tmp_Users ON
GO
IF EXISTS(SELECT * FROM dbo.Users)
	 EXEC('INSERT INTO dbo.Tmp_Users (UserRno, UserName, UserCommonName, PasswordSalt, Password, AdminFlg, DisabledFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser)
		SELECT UserRno, UserName, UserCommonName, PasswordSalt, Password, AdminFlg, DisabledFlg, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser FROM dbo.Users WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Users OFF
GO
DROP TABLE dbo.Users
GO
EXECUTE sp_rename N'dbo.Tmp_Users', N'Users', 'OBJECT' 
GO
ALTER TABLE dbo.Users ADD CONSTRAINT
	PK_Users PRIMARY KEY CLUSTERED 
	(
	UserRno
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Users ADD CONSTRAINT
	IX_Users UNIQUE NONCLUSTERED 
	(
	UserName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
