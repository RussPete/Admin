drop table Ingredients

/****** Object:  Table [dbo].[Ingredients]    Script Date: 7/22/2013 9:21:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Ingredients](
	[IngredRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[DryLiquid] [char](1) NULL,
	[StockedFlg] [bit] NULL,
	[PrefBrands] [varchar](1000) NULL,
	[RejBrands] [varchar](1000) NULL,
	[PrefVendors] [varchar](100) NULL,
	[RejVendors] [varchar](100) NULL,
	[IntNote] [varchar](2000) NULL,
	[CT_ItemID] [int] NULL,
	[HideFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Ingredients] PRIMARY KEY CLUSTERED 
(
	[IngredRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


