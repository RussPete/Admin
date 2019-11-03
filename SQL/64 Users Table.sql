USE [MarvellousCatering]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 10/20/2015 9:48:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Users](
	[UserRno] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](20) NOT NULL,
	[UserCommonName] [varchar](50) NULL,
	[PasswordSalt] [varchar](64) NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[AdminFlg] [bit] NOT NULL CONSTRAINT [DF_Users_AdminFlg]  DEFAULT ((0)),
	[DisabledFlg] [bit] NOT NULL CONSTRAINT [DF_Users_DisabledFlg]  DEFAULT ((0)),
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Users] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

