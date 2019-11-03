
/****** Object:  Table [dbo].[VendorIngredXref]    Script Date: 7/22/2013 9:22:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[VendorIngredXref](
	[VendorIngredRno] [int] IDENTITY(1,1) NOT NULL,
	[VendorRno] [int] NOT NULL,
	[IngredRno] [int] NOT NULL,
	[PurchaseQty] [decimal](18, 4) NULL,
	[PurchaseUnitRno] [int] NULL,
	[PrefOrder] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_VendorIngredXref] PRIMARY KEY CLUSTERED 
(
	[VendorIngredRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


