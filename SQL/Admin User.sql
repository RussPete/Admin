-- add a user name 'admin' with password 'pass'

USE [MarvellousCatering]
GO

INSERT INTO [dbo].[Users]
           ([UserName]
           ,[UserCommonName]
           ,[PasswordSalt]
           ,[Password]
           ,[AdminFlg]
           ,[DisabledFlg]
           ,[CreatedDtTm]
           ,[CreatedUser]
           ,[UpdatedDtTm]
           ,[UpdatedUser])
     VALUES
           ('admin'
           ,NULL
           ,'E7D1ABDEAC670D6A10E36BEF5D2E77ADC89F372F809098C0F4F7CC0E7BA9E227'
           ,'65b8b2ffa179280066835ab34949ce38f51c21c12953f3083ec78ffe778d3520'
           ,1
           ,0
           ,NULL
           ,NULL
           ,NULL
           ,NULL)
GO
