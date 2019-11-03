
/****** Object:  Table [dbo].[Purchases]    Script Date: 8/24/2013 11:37:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Purchases](
	[PurchaseRno] [int] IDENTITY(1,1) NOT NULL,
	[VendorRno] [int] NULL,
	[PurchaseDt] [datetime] NULL,
	[PurchaseOrdNum] [varchar](20) NULL,
	[VendorInvNum] [varchar](20) NULL,
	[Note] [varchar](2000) NULL,
	[CT_InvoiceID] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED 
(
	[PurchaseRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


