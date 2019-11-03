/* create EventTypes table */
/****** Object:  Table [dbo].[EventTypes]    Script Date: 11/15/2013 4:18:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EventTypes](
	[EventTypeRno] [int] NOT NULL,
	[EventType] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[HideFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


