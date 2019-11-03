-- create Contacts and Customers

/****** Object:  Table [dbo].[Contacts]    Script Date: 12/26/2018 3:40:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contacts](
	[ContactRno] [int] IDENTITY(1,1) NOT NULL,
	[CustomerRno] [int] NULL,
	[Name] [varchar](50) NULL,
	[Phone] [varchar](20) NULL,
	[Cell] [varchar](20) NULL,
	[Fax] [varchar](20) NULL,
	[Email] [varchar](80) NULL,
	[Title] [varchar](30) NULL,
	[ExportDtTm] [datetime] NULL,
	[ExportId] [varchar](36) NULL,
	[InactiveDtTm] [datetime] NULL,
	[InactiveUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[ContactRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Customers]    Script Date: 12/26/2018 3:41:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customers](
	[CustomerRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[TaxExemptFlg] [tinyint] NULL,
	[CompanyFlg] [tinyint] NULL,
	[OnlineOrderFlg] [tinyint] NULL,
	[EmailConfirmationsFlg] [tinyint] NULL,
	[AnnualContractSignedName] [varchar](50) NULL,
	[AnnualContractSignedDtTm] [datetime] NULL,
	[InactiveDtTm] [datetime] NULL,
	[InactiveUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
