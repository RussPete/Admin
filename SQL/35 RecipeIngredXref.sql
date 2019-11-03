
/****** Object:  Table [dbo].[RecipeIngredXref]    Script Date: 7/22/2013 9:22:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RecipeIngredXref](
	[RecipeIngredRno] [int] IDENTITY(1,1) NOT NULL,
	[RecipeRno] [int] NOT NULL,
	[RecipeSeq] [int] NOT NULL,
	[IngredRno] [int] NULL,
	[SubRecipeRno] [int] NULL,
	[UnitQty] [decimal](18, 4) NULL,
	[UnitRno] [int] NULL,
	[Note] [varchar](2000) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_RecipeIngredXref] PRIMARY KEY CLUSTERED 
(
	[RecipeIngredRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


