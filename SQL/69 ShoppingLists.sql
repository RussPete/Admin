/* create ShoppingLists & ShoppingListDetails */

GO

/****** Object:  Table [dbo].[ShoppingLists]    Script Date: 12/13/2016 1:40:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ShoppingLists](
	[ShoppingListRno] [int] IDENTITY(1,1) NOT NULL,
	[VendorRno] [int] NOT NULL,
	[BegDt] [datetime] NOT NULL,
	[EndDt] [datetime] NOT NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_ShoppingLists] PRIMARY KEY CLUSTERED 
(
	[ShoppingListRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


GO

/****** Object:  Table [dbo].[ShoppingListDetails]    Script Date: 12/13/2016 1:41:08 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ShoppingListDetails](
	[ShoppingListDetailRno] [int] IDENTITY(1,1) NOT NULL,
	[ShoppingListRno] [int] NOT NULL,
	[IngredRno] [int] NOT NULL,
	[PurchaseQty] [decimal](18, 4) NULL,
	[PurchaseUnitQty] [decimal](18, 4) NULL,
	[PurchaseUnitRno] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_ShoppingListDetails] PRIMARY KEY CLUSTERED 
(
	[ShoppingListDetailRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

