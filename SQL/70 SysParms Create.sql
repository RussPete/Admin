/* create table */

/****** Object:  Table [dbo].[SysParms]    Script Date: 12/25/2016 11:26:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SysParms](
	[SysParmRno] [int] IDENTITY(1,1) NOT NULL,
	[SysParmName] [varchar](50) NOT NULL,
	[SysParmValue] [varchar](500) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_SysParms] PRIMARY KEY CLUSTERED 
(
	[SysParmRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


