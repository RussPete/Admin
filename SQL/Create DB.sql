--  change CaterDB to the new DB name

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CatererDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CatererDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CatererDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CatererDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CatererDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CatererDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [CatererDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CatererDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CatererDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CatererDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CatererDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CatererDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CatererDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CatererDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CatererDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CatererDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CatererDB] SET AUTO_UPDATE_STATISTICS_ASYNC ON 
GO
ALTER DATABASE [CatererDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CatererDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CatererDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CatererDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CatererDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CatererDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CatererDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CatererDB] SET  MULTI_USER 
GO
ALTER DATABASE [CatererDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CatererDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CatererDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CatererDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [CatererDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'MarvellousCate', N'ON'
GO
ALTER DATABASE [CatererDB] SET QUERY_STORE = OFF
GO
USE [CatererDB]
GO
/****** Object:  User [CatererDB]    Script Date: 10/25/2019 8:20:31 AM ******/
CREATE USER [CatererDB] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [CatererDB]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [CatererDB]
GO
ALTER ROLE [db_datareader] ADD MEMBER [CatererDB]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [CatererDB]
GO
/****** Object:  FullTextCatalog [CatererDB]    Script Date: 10/25/2019 8:20:31 AM ******/
CREATE FULLTEXT CATALOG [CatererDB] AS DEFAULT
GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 10/25/2019 8:20:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ContactRno] [int] IDENTITY(1,1) NOT NULL,
	[CustomerRno] [int] NULL,
	[Name] [varchar](50) NULL,
	[Phone] [varchar](20) NULL,
	[Cell] [varchar](20) NULL,
	[Fax] [varchar](20) NULL,
	[Email] [varchar](80) NULL,
	[Title] [varchar](30) NULL,
	[ExportDtTm] [datetime] NULL,
	[ExportId] [varchar](36) NULL,
	[InactiveDtTm] [datetime] NULL,
	[InactiveUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[ContactRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 10/25/2019 8:20:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[TaxExemptFlg] [tinyint] NULL,
	[CompanyFlg] [tinyint] NULL,
	[OnlineOrderFlg] [tinyint] NULL,
	[EmailConfirmationsFlg] [tinyint] NULL,
	[AnnualContractSignedName] [varchar](50) NULL,
	[AnnualContractSignedDtTm] [datetime] NULL,
	[InactiveDtTm] [datetime] NULL,
	[InactiveUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventTypes]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[IngredConv]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredConv](
	[IngredConvRno] [int] IDENTITY(1,1) NOT NULL,
	[IngredRno] [int] NOT NULL,
	[PurchaseQty] [decimal](18, 4) NULL,
	[PurchaseUnitRno] [int] NULL,
	[RecipeQty] [decimal](18, 4) NULL,
	[RecipeUnitRno] [int] NULL,
	[MissingFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_IngredConv] PRIMARY KEY CLUSTERED 
(
	[IngredConvRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientCategories]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientCategories](
	[Category] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_IngredientCategories] PRIMARY KEY CLUSTERED 
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ingredients]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ingredients](
	[IngredRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[DryLiquid] [char](1) NULL,
	[StockedFlg] [bit] NULL,
	[StockedPurchaseQty] [decimal](18, 4) NULL,
	[PrefBrands] [varchar](1000) NULL,
	[RejBrands] [varchar](1000) NULL,
	[PrefVendors] [varchar](100) NULL,
	[RejVendors] [varchar](100) NULL,
	[IntNote] [varchar](2000) NULL,
	[CT_ItemID] [int] NULL,
	[NonPurchaseFlg] [bit] NULL,
	[MenuItemAsIsFlg] [bit] NULL,
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
/****** Object:  Table [dbo].[JobInvoicePrices]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobInvoicePrices](
	[Rno] [int] NULL,
	[JobRno] [int] NULL,
	[Seq] [int] NULL,
	[PriceType] [varchar](20) NULL,
	[InvPerPersonCount] [int] NULL,
	[InvPerPersonDesc] [varchar](50) NULL,
	[InvPerPersonPrice] [money] NULL,
	[InvPerPersonExtPrice] [money] NULL,
	[InvPerItemCount] [int] NULL,
	[InvPerItemDesc] [varchar](50) NULL,
	[InvPerItemPrice] [money] NULL,
	[InvPerItemExtPrice] [money] NULL,
	[InvAllInclDesc] [varchar](50) NULL,
	[InvAllInclPrice] [money] NULL,
	[InvAllInclExtPrice] [money] NULL,
	[PropPerPersonDesc] [varchar](1000) NULL,
	[PropPerPersonDescCustFlg] [bit] NULL,
	[PropPerPersonOptions] [varchar](200) NULL,
	[PropPerItemDesc] [varchar](1000) NULL,
	[PropPerItemDescCustFlg] [bit] NULL,
	[PropAllInclDesc] [varchar](1000) NULL,
	[PropAllInclDescCustFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobLinens]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobLinens](
	[LinenRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
	[Caterer] [varchar](60) NULL,
	[Other] [varchar](60) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobLinens] PRIMARY KEY CLUSTERED 
(
	[LinenRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobTables]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobTables](
	[TableRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
	[Caterer] [varchar](60) NULL,
	[Other] [varchar](60) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobTables] PRIMARY KEY CLUSTERED 
(
	[TableRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KitchenLocations]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KitchenLocations](
	[KitchenLocRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[DefaultFlg] [bit] NULL,
	[HideFlg] [bit] NULL,
 CONSTRAINT [PK_KitchenLocations] PRIMARY KEY CLUSTERED 
(
	[KitchenLocRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcErrors]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcErrors](
	[DtTm] [datetime] NULL,
	[Message] [varchar](1024) NULL,
	[SrcFile] [varchar](32) NULL,
	[SrcFunct] [varchar](64) NULL,
	[SrcLine] [int] NULL,
	[Sql] [varchar](4096) NULL,
	[Stack] [varchar](4096) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobCrew]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobCrew](
	[JobRno] [int] NOT NULL,
	[CrewSeq] [int] NOT NULL,
	[CrewMember] [varchar](50) NULL,
	[CrewAssignment] [varchar](50) NULL,
	[ReportTime] [datetime] NULL,
	[Note] [varchar](128) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobCrew] PRIMARY KEY CLUSTERED 
(
	[JobRno] ASC,
	[CrewSeq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobDishes]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobDishes](
	[DishRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[DishSeq] [int] NOT NULL,
	[DishItem] [varchar](50) NULL,
	[Qty] [int] NULL,
	[Note] [varchar](128) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_mcJobDishes] PRIMARY KEY CLUSTERED 
(
	[DishRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobFood]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobFood](
	[JobFoodRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[FoodSeq] [int] NULL,
	[Category] [varchar](50) NULL,
	[MenuItem] [varchar](50) NULL,
	[MenuItemRno] [int] NULL,
	[Qty] [int] NULL,
	[QtyNote] [varchar](128) NULL,
	[ServiceNote] [varchar](128) NULL,
	[ProposalSeq] [int] NULL,
	[ProposalMenuItem] [varchar](1000) NULL,
	[ProposalTitleFlg] [bit] NULL,
	[ProposalHideFlg] [bit] NOT NULL,
	[IngredSelFlg] [bit] NULL,
	[IngredSel] [varchar](1000) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobFood] PRIMARY KEY CLUSTERED 
(
	[JobFoodRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobMenuCategories]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobMenuCategories](
	[Category] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[MultSelFlg] [bit] NULL,
	[HideFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_mcJobMenuCategories] PRIMARY KEY CLUSTERED 
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobMenuItems]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobMenuItems](
	[MenuItemRno] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](50) NULL,
	[MenuItem] [varchar](50) NULL,
	[ProposalMenuItem] [varchar](100) NULL,
	[KitchenLocRno] [int] NULL,
	[HotFlg] [bit] NULL,
	[ColdFlg] [bit] NULL,
	[HideFlg] [bit] NULL,
	[AsIsFlg] [bit] NULL,
	[MultSelFlg] [bit] NULL,
	[MultItems] [varchar](4096) NULL,
	[IngredSelFlg] [bit] NULL,
	[CategorySortOrder] [int] NULL,
	[RecipeRno] [int] NULL,
	[ServingQuote] [money] NULL,
	[ServingPrice] [money] NULL,
	[InaccuratePriceFlg] [bit] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_mcJobMenuItems] PRIMARY KEY CLUSTERED 
(
	[MenuItemRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobs]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobs](
	[JobRno] [int] NOT NULL,
	[JobDesc] [varchar](80) NULL,
	[ContactRno] [int] NULL,
	[ContractSendDtTm] [datetime] NULL,
	[ContractSignedFileName] [varchar](200) NULL,
	[ResponsibleParty] [varchar](50) NULL,
	[BookedBy] [varchar](50) NULL,
	[ConfirmedBy] [varchar](50) NULL,
	[JobType] [varchar](20) NULL,
	[PmtTerms] [varchar](20) NULL,
	[ProposalFlg] [bit] NOT NULL,
	[EventType] [varchar](50) NULL,
	[EventTypeDetails] [varchar](200) NULL,
	[ServiceType] [varchar](50) NULL,
	[Location] [varchar](80) NULL,
	[LocationDirections] [varchar](1024) NULL,
	[PrintDirectionsFlg] [tinyint] NULL,
	[Vehicle] [varchar](50) NULL,
	[Carts] [varchar](50) NULL,
	[NumBuffets] [int] NULL,
	[BuffetSpace] [varchar](200) NULL,
	[JobDate] [datetime] NULL,
	[LoadTime] [datetime] NULL,
	[DepartureTime] [datetime] NULL,
	[ArrivalTime] [datetime] NULL,
	[GuestArrivalTime] [datetime] NULL,
	[MealTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[CeremonyTime] [datetime] NULL,
	[CeremonyFlg] [bit] NULL,
	[CeremonyLocation] [varchar](80) NULL,
	[NumMenServing] [int] NULL,
	[NumWomenServing] [int] NULL,
	[NumChildServing] [int] NULL,
	[PricePerPerson] [money] NULL,
	[PmtMethod] [varchar](20) NULL,
	[MenuType] [varchar](30) NULL,
	[PONumber] [varchar](50) NULL,
	[Demographics] [varchar](1024) NULL,
	[JobNotes] [varchar](4096) NULL,
	[ProductionNotes] [varchar](4096) NULL,
	[Crew] [varchar](200) NULL,
	[Status] [varchar](20) NULL,
	[ConfirmDeadlineDate] [datetime] NULL,
	[InvDate] [datetime] NULL,
	[InvEventCount] [int] NULL,
	[InvPricePerPerson] [money] NULL,
	[SubTotAmt] [money] NULL,
	[SubTotTaxPct] [decimal](18, 4) NULL,
	[ServiceSubTotPctFlg] [bit] NULL,
	[ServiceSubTotPct] [decimal](18, 4) NULL,
	[ServiceAmt] [money] NULL,
	[ServiceTaxPct] [decimal](18, 4) NULL,
	[ServicePropDesc] [varchar](1000) NULL,
	[ServicePropDescCustFlg] [bit] NULL,
	[ServicePropOptions] [varchar](200) NULL,
	[DeliverySubTotPctFlg] [bit] NULL,
	[DeliverySubTotPct] [decimal](18, 4) NULL,
	[DeliveryAmt] [money] NULL,
	[DeliveryTaxPct] [decimal](18, 4) NULL,
	[DeliveryPropDesc] [varchar](1000) NULL,
	[DeliveryPropDescCustFlg] [bit] NULL,
	[DeliveryPropOptions] [varchar](200) NULL,
	[ChinaDesc] [varchar](100) NULL,
	[ChinaSubTotPctFlg] [bit] NULL,
	[ChinaSubTotPct] [decimal](18, 4) NULL,
	[ChinaAmt] [money] NULL,
	[ChinaTaxPct] [decimal](18, 4) NULL,
	[ChinaPropDesc] [varchar](1000) NULL,
	[ChinaPropDescCustFlg] [bit] NULL,
	[ChinaPropOptions] [varchar](200) NULL,
	[AddServiceSubTotPctFlg] [bit] NULL,
	[AddServiceSubTotPct] [decimal](18, 4) NULL,
	[AddServiceAmt] [money] NULL,
	[AddServiceTaxPct] [decimal](18, 4) NULL,
	[AddServicePropDesc] [varchar](1000) NULL,
	[AddServicePropDescCustFlg] [bit] NULL,
	[AddServicePropOptions] [varchar](200) NULL,
	[FuelTravelSubTotPctFlg] [bit] NULL,
	[FuelTravelSubTotPct] [decimal](18, 4) NULL,
	[FuelTravelAmt] [money] NULL,
	[FuelTravelTaxPct] [decimal](18, 4) NULL,
	[FuelTravelPropDesc] [varchar](1000) NULL,
	[FuelTravelPropDescCustFlg] [bit] NULL,
	[FuelTravelPropOptions] [varchar](200) NULL,
	[FacilitySubTotPctFlg] [bit] NULL,
	[FacilitySubTotPct] [decimal](18, 4) NULL,
	[FacilityAmt] [money] NULL,
	[FacilityTaxPct] [decimal](18, 4) NULL,
	[FacilityPropDesc] [varchar](1000) NULL,
	[FacilityPropDescCustFlg] [bit] NULL,
	[FacilityPropOptions] [varchar](200) NULL,
	[GratuitySubTotPctFlg] [bit] NULL,
	[GratuitySubTotPct] [decimal](18, 4) NULL,
	[GratuityAmt] [money] NULL,
	[GratuityTaxPct] [decimal](18, 4) NULL,
	[VoluntaryGratuitySubTotPctFlg] [bit] NULL,
	[VoluntaryGratuitySubTotPct] [decimal](18, 4) NULL,
	[VoluntaryGratuityAmt] [money] NULL,
	[RentalsDesc] [varchar](100) NULL,
	[RentalsSubTotPctFlg] [bit] NULL,
	[RentalsSubTotPct] [decimal](18, 4) NULL,
	[RentalsAmt] [money] NULL,
	[RentalsTaxPct] [decimal](18, 4) NULL,
	[RentalsPropDesc] [varchar](1000) NULL,
	[RentalsPropDescCustFlg] [bit] NULL,
	[RentalsPropOptions] [varchar](200) NULL,
	[Adj1Desc] [varchar](50) NULL,
	[Adj1SubTotPctFlg] [bit] NULL,
	[Adj1SubTotPct] [decimal](18, 4) NULL,
	[Adj1Amt] [money] NULL,
	[Adj1TaxPct] [decimal](18, 4) NULL,
	[Adj1PropDesc] [varchar](1000) NULL,
	[Adj2Desc] [varchar](50) NULL,
	[Adj2SubTotPctFlg] [bit] NULL,
	[Adj2SubTotPct] [decimal](18, 4) NULL,
	[Adj2Amt] [money] NULL,
	[Adj2TaxPct] [decimal](18, 4) NULL,
	[Adj2PropDesc] [varchar](1000) NULL,
	[EstTotPropDesc] [varchar](1000) NULL,
	[EstTotPropDescCustFlg] [bit] NULL,
	[EstTotPropOptions] [varchar](200) NULL,
	[DepositPropDesc] [varchar](1000) NULL,
	[DepositPropDescCustFlg] [bit] NULL,
	[DepositPropOptions] [varchar](200) NULL,
	[InvoiceMsg] [varchar](200) NULL,
	[InvDeliveryMethod] [varchar](20) NULL,
	[InvDeliveryDate] [datetime] NULL,
	[InvEmail] [varchar](80) NULL,
	[InvFax] [varchar](30) NULL,
	[DepositAmt] [money] NULL,
	[CCPmtFeeFlg] [bit] NULL,
	[CCFeePct] [decimal](18, 4) NULL,
	[CCFeeAmt] [money] NULL,
	[CCFeeTaxPct] [decimal](18, 4) NULL,
	[PreTaxSubTotAmt] [money] NULL,
	[SalesTaxTotAmt] [money] NULL,
	[InvTotAmt] [money] NULL,
	[InvBalAmt] [money] NULL,
	[MCCLinens] [varchar](200) NULL,
	[DiamondLinens] [varchar](200) NULL,
	[SusanLinens] [varchar](200) NULL,
	[CUELinens] [varchar](200) NULL,
	[Shirts] [varchar](50) NULL,
	[Aprons] [varchar](50) NULL,
	[DisposableFlg] [bit] NULL,
	[SlingShiftId] [bigint] NULL,
	[FinalDtTm] [datetime] NULL,
	[FinalUser] [varchar](20) NULL,
	[ProcessedDtTm] [datetime] NULL,
	[ProcessedUser] [varchar](20) NULL,
	[EditProcessedDtTm] [datetime] NULL,
	[EditProcessedUser] [varchar](20) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
	[InvCreatedDtTm] [datetime] NULL,
	[InvCreatedUser] [varchar](20) NULL,
	[InvUpdatedDtTm] [datetime] NULL,
	[InvUpdatedUser] [varchar](20) NULL,
	[CompletedDtTm] [datetime] NULL,
	[CompletedUser] [varchar](20) NULL,
	[FinishConfirmDtTm] [datetime] NULL,
	[ConfirmedDtTm] [datetime] NULL,
	[ConfirmedUser] [varchar](20) NULL,
	[PrintedDtTm] [datetime] NULL,
	[PrintedUser] [varchar](20) NULL,
	[ExportDtTm] [datetime] NULL,
	[ExportId] [varchar](36) NULL,
	[InvoicedDtTm] [datetime] NULL,
	[InvoicedUser] [varchar](20) NULL,
	[CancelledDtTm] [datetime] NULL,
	[CancelledUser] [varchar](20) NULL,
	[InvEmailedDtTm] [datetime] NULL,
	[InvEmailedUser] [varchar](20) NULL,
	[PropCreatedDtTm] [datetime] NULL,
	[PropCreatedUser] [varchar](20) NULL,
	[PropUpdatedDtTm] [datetime] NULL,
	[PropUpdatedUser] [varchar](20) NULL,
	[PropGeneratedDtTm] [datetime] NULL,
	[PropGeneratedUser] [varchar](20) NULL,
	[PropEmailedDtTm] [datetime] NULL,
	[PropEmailedUser] [varchar](20) NULL,
	[ConfirmEmailedDtTm] [datetime] NULL,
	[ConfirmEmailedUser] [varchar](20) NULL,
	[FinalConfirmEmailedDtTm] [datetime] NULL,
	[FinalConfirmEmailedUser] [varchar](20) NULL,
	[DepositInvPrintedDtTm] [datetime] NULL,
	[DepositInvPrintedUser] [varchar](20) NULL,
	[DepositInvEmailedDtTm] [datetime] NULL,
	[DepositInvEmailedUser] [varchar](20) NULL,
	[PaidInFullDtTm] [datetime] NULL,
	[PaidInFullUser] [varchar](20) NULL,
	[Guid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobServices]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobServices](
	[ServiceRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[ServiceSeq] [int] NOT NULL,
	[ServiceItem] [varchar](50) NULL,
	[MCCRespFlg] [tinyint] NULL,
	[CustomerRespFlg] [tinyint] NULL,
	[Note] [varchar](128) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobServices] PRIMARY KEY CLUSTERED 
(
	[ServiceRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mcJobSupplies]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mcJobSupplies](
	[SupplyRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[SupplySeq] [int] NOT NULL,
	[SupplyItem] [varchar](50) NULL,
	[Qty] [int] NULL,
	[Note] [varchar](128) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_JobSupplies] PRIMARY KEY CLUSTERED 
(
	[SupplyRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentRno] [int] IDENTITY(1,1) NOT NULL,
	[JobRno] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[Amount] [money] NULL,
	[PaymentDt] [datetime] NULL,
	[Reference] [varchar](130) NULL,
	[Method] [varchar](12) NULL,
	[Type] [varchar](12) NULL,
	[PurchaseOrder] [varchar](50) NULL,
	[CCReqAmt] [money] NULL,
	[CCType] [varchar](20) NULL,
	[CCNum] [varchar](100) NULL,
	[CCExpDt] [varchar](6) NULL,
	[CCSecCode] [varchar](50) NULL,
	[CCName] [varchar](50) NULL,
	[CCAddr] [varchar](50) NULL,
	[CCZip] [varchar](15) NULL,
	[CCStatus] [varchar](30) NULL,
	[CCResult] [varchar](150) NULL,
	[CCAuth] [varchar](10) NULL,
	[CCReference] [varchar](130) NULL,
	[CCPmtDtTm] [datetime] NULL,
	[CCPmtUser] [varchar](20) NULL,
	[CCPrintDtTm] [datetime] NULL,
	[CCPrintUser] [varchar](20) NULL,
	[ExportDtTm] [datetime] NULL,
	[ExportId] [varchar](36) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProposalPricingOptions]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposalPricingOptions](
	[PropPricingRno] [int] IDENTITY(1,1) NOT NULL,
	[PricingCode] [varchar](20) NULL,
	[Seq] [int] NULL,
	[OptionType] [varchar](10) NULL,
	[OptionCheckboxDesc] [varchar](50) NULL,
	[OptionPropDesc] [varchar](50) NULL,
	[OptionDefault] [varchar](50) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
	[DeletedDtTm] [datetime] NULL,
	[DeletedUser] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseDetails]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[Purchases]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchases](
	[PurchaseRno] [int] IDENTITY(1,1) NOT NULL,
	[VendorRno] [int] NULL,
	[PurchaseDt] [datetime] NULL,
	[PurchaseOrdNum] [varchar](20) NULL,
	[VendorInvNum] [varchar](20) NULL,
	[Note] [varchar](2000) NULL,
	[CT_InvoiceID] [int] NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED 
(
	[PurchaseRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipeIngredXref]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipeIngredXref](
	[RecipeIngredRno] [int] IDENTITY(1,1) NOT NULL,
	[RecipeRno] [int] NOT NULL,
	[RecipeSeq] [int] NOT NULL,
	[IngredRno] [int] NULL,
	[SubrecipeRno] [int] NULL,
	[UnitQty] [decimal](18, 4) NULL,
	[UnitRno] [int] NULL,
	[BaseCostPrice] [decimal](18, 4) NULL,
	[Title] [varchar](500) NULL,
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
/****** Object:  Table [dbo].[Recipes]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Recipes](
	[RecipeRno] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Category] [varchar](50) NULL,
	[SubrecipeFlg] [bit] NULL,
	[NumServings] [decimal](18, 4) NULL,
	[MenServingRatio] [decimal](18, 4) NULL,
	[WomenServingRatio] [decimal](18, 4) NULL,
	[ChildServingRatio] [decimal](18, 4) NULL,
	[YieldQty] [decimal](18, 4) NULL,
	[YieldUnitRno] [int] NULL,
	[PortionQty] [decimal](18, 4) NULL,
	[PortionUnitRno] [int] NULL,
	[PortionPrice] [decimal](18, 4) NULL,
	[BaseCostPrice] [decimal](18, 4) NULL,
	[BaseCostPct] [decimal](18, 4) NULL,
	[InaccuratePriceFlg] [bit] NULL,
	[Instructions] [varchar](8000) NULL,
	[Source] [varchar](1000) NULL,
	[Images] [varchar](4000) NULL,
	[IntNote] [varchar](2000) NULL,
	[CT_RecipeID] [int] NULL,
	[MenuItemAsIsFlg] [bit] NULL,
	[GlutenFreeFlg] [bit] NULL,
	[VeganFlg] [bit] NULL,
	[VegetarianFlg] [bit] NULL,
	[DairyFreeFlg] [bit] NULL,
	[NutsFlg] [bit] NULL,
	[HideFlg] [bit] NULL,
	[IncludeInBookFlg] [bit] NULL,
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
/****** Object:  Table [dbo].[Settings]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingRno] [int] NOT NULL,
	[SubTotTaxPct] [decimal](18, 4) NULL,
	[ServiceSubTotPct] [decimal](18, 4) NULL,
	[ServiceTaxPct] [decimal](18, 4) NULL,
	[DeliverySubTotPct] [decimal](18, 4) NULL,
	[DeliveryTaxPct] [decimal](18, 4) NULL,
	[ChinaSubTotPct] [decimal](18, 4) NULL,
	[ChinaTaxPct] [decimal](18, 4) NULL,
	[AddServiceSubTotPct] [decimal](18, 4) NULL,
	[AddServiceTaxPct] [decimal](18, 4) NULL,
	[FuelTravelSubTotPct] [decimal](18, 4) NULL,
	[FuelTravelTaxPct] [decimal](18, 4) NULL,
	[FacilitySubTotPct] [decimal](18, 4) NULL,
	[FacilityTaxPct] [decimal](18, 4) NULL,
	[GratuitySubTotPct] [decimal](18, 4) NULL,
	[GratuityTaxPct] [decimal](18, 4) NULL,
	[RentalsSubTotPct] [decimal](18, 4) NULL,
	[RentalsTaxPct] [decimal](18, 4) NULL,
	[Adj1TaxPct] [decimal](18, 4) NULL,
	[Adj2TaxPct] [decimal](18, 4) NULL,
	[CCFeePct] [decimal](18, 4) NULL,
	[CCFeeTaxPct] [decimal](18, 4) NULL,
	[BaseCostPct] [decimal](18, 4) NULL,
	[AsIsBaseCostPct] [decimal](18, 4) NULL,
	[QuoteDiffPct] [decimal](18, 4) NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShoppingListDetails]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[ShoppingLists]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[SysParms]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[Units]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Units](
	[UnitRno] [int] IDENTITY(1,1) NOT NULL,
	[UnitSingle] [varchar](20) NULL,
	[UnitPlural] [varchar](20) NULL,
	[HideFlg] [bit] NULL,
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
/****** Object:  Table [dbo].[Users]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserRno] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](20) NOT NULL,
	[UserCommonName] [varchar](50) NULL,
	[PasswordSalt] [varchar](64) NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[AccessLevel] [int] NOT NULL,
	[AdminFlg] [bit] NOT NULL,
	[DisabledFlg] [bit] NOT NULL,
	[CreatedDtTm] [datetime] NULL,
	[CreatedUser] [varchar](20) NULL,
	[UpdatedDtTm] [datetime] NULL,
	[UpdatedUser] [varchar](20) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Users] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VendorIngredXref]    Script Date: 10/25/2019 8:20:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[Vendors]    Script Date: 10/25/2019 8:20:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Index [IX_JobLinens_Jobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [IX_JobLinens_Jobs] ON [dbo].[JobLinens]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobTables_Jobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [IX_JobTables_Jobs] ON [dbo].[JobTables]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [Mem_mcJobCrew]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Mem_mcJobCrew] ON [dbo].[mcJobCrew]
(
	[CrewMember] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobDishes_Jobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [IX_JobDishes_Jobs] ON [dbo].[mcJobDishes]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [Cat_mcJobFood]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Cat_mcJobFood] ON [dbo].[mcJobFood]
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [Item_mcJobFood]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Item_mcJobFood] ON [dbo].[mcJobFood]
(
	[MenuItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Contact_mcJobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Contact_mcJobs] ON [dbo].[mcJobs]
(
	[ContactRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Date_mcJobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Date_mcJobs] ON [dbo].[mcJobs]
(
	[JobDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Guid_mcJobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [Guid_mcJobs] ON [dbo].[mcJobs]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobServices_Jobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [IX_JobServices_Jobs] ON [dbo].[mcJobServices]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_JobSupplies_Jobs]    Script Date: 10/25/2019 8:20:33 AM ******/
CREATE NONCLUSTERED INDEX [IX_JobSupplies_Jobs] ON [dbo].[mcJobSupplies]
(
	[JobRno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[mcJobFood] ADD  CONSTRAINT [DF_mcJobFood_ProposalHideFlg]  DEFAULT ((0)) FOR [ProposalHideFlg]
GO
ALTER TABLE [dbo].[mcJobs] ADD  CONSTRAINT [DF_mcJobs_ProposalFlg]  DEFAULT ((0)) FOR [ProposalFlg]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_AccessLevel]  DEFAULT ((0)) FOR [AccessLevel]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_AdminFlg]  DEFAULT ((0)) FOR [AdminFlg]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_DisabledFlg]  DEFAULT ((0)) FOR [DisabledFlg]
GO
USE [master]
GO
ALTER DATABASE [CatererDB] SET  READ_WRITE 
GO
