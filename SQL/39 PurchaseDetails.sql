
/****** Object:  Table [dbo].[PurchaseDetails]    Script Date: 8/24/2013 11:37:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PurchaseDetails](
	[PurchaseDetailRno] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseRno] [int] NULL,
	[IngredRno] [int] NULL,
	[PurchaseQty] [decimal](18, 4) NULL,
	[PurchaseUnitQty] [decimal](18, 4) NULL,
	[PurchaseUnitRno] [int] NULL,
	[Price] [decimal](18, 4) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_PurchaseDetails] PRIMARY KEY CLUSTERED 
(
	[PurchaseDetailRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


