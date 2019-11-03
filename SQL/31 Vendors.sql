/****** Object:  Table [dbo].[Vendors]    Script Date: 8/11/2013 3:01:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Vendors](
	[VendorRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Phone] [varchar](20) NULL,
	[Contact] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[Website] [varchar](100) NULL,
	[Note] [varchar](2000) NULL,
	[HideFlg] [bit] NULL,
	[CT_VendorID] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[VendorRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


