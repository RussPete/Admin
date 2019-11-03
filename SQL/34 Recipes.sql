Drop table Recipes

/****** Object:  Table [dbo].[Recipes]    Script Date: 7/22/2013 9:22:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Recipes](
	[RecipeRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[SubRecipeFlg] [bit] NULL,
	[NumServings] [decimal](18, 4) NULL,
	[MenServingRatio] [decimal](18, 4) NULL,
	[WomenServingRatio] [decimal](18, 4) NULL,
	[ChildServingRatio] [decimal](18, 4) NULL,
	[YieldQty] [decimal](18, 4) NULL,
	[YieldUnitRno] [int] NULL,
	[PortionQty] [decimal](18, 4) NULL,
	[PortionUnitRno] [int] NULL,
	[CostBasePct] [decimal](18, 4) NULL,
	[Instructions] [varchar](8000) NULL,
	[Source] [varchar](1000) NULL,
	[Images] [varchar](4000) NULL,
	[IntNote] [varchar](2000) NULL,
	[CT_RecipeID] [int] NULL,
	[HideFlg] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Recipes] PRIMARY KEY CLUSTERED 
(
	[RecipeRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


