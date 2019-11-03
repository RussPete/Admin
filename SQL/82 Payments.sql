/* create Payments table */

/****** Object:  Table [dbo].[Payments]    Script Date: 9/28/2017 8:18:57 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Payments](
	[PaymentRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[Amount] [money] NULL,
	[PaymentDt] [datetime] NULL,
	[Reference] [varchar](130) NULL,
	[Method] [varchar](12) NULL,
	[Type] [varchar](12) NULL,
	[PurchaseOrder] [varchar](50) NULL,
	[CCReqAmt] [money] NULL,
	[CCType] [varchar](20) NULL,
	[CCNum] [varchar](100) NULL,
	[CCExpDt] [varchar](6) NULL,
	[CCSecCode] [varchar](50) NULL,
	[CCName] [varchar](50) NULL,
	[CCAddr] [varchar](50) NULL,
	[CCZip] [varchar](15) NULL,
	[CCStatus] [varchar](30) NULL,
	[CCResult] [varchar](150) NULL,
	[CCAuth] [varchar](10) NULL,
	[CCReference] [varchar](130) NULL,
	[CCPmtDtTm] [datetime] NULL,
	[CCPmtUser] [varchar](20) NULL,
	[CCPrintDtTm] [datetime] NULL,
	[CCPrintUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


