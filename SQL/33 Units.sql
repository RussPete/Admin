
/****** Object:  Table [dbo].[Units]    Script Date: 7/22/2013 9:22:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Units](
	[UnitRno] [int] IDENTITY(1,1) NOT NULL,
	[UnitSingle] [varchar](20) NULL,
	[UnitPlural] [varchar](20) NULL,
	[CT_UnitID] [smallint] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[UnitRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


