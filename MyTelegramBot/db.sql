USE [master]
GO
/****** Object:  Database [MarketBotDb]    Script Date: 03.12.2017 21:49:06 ******/
CREATE DATABASE [MarketBotDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MarketBotDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\MarketBotDb.mdf' , SIZE = 7168KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ), 
 FILEGROUP [BotDbFs] CONTAINS FILESTREAM  DEFAULT
( NAME = N'MarketBotDb_filestream', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\MarketBotDb_filestream' , MAXSIZE = UNLIMITED)
 LOG ON 
( NAME = N'MarketBotDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\MarketBotDb_log.ldf' , SIZE = 6912KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MarketBotDb] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MarketBotDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MarketBotDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MarketBotDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MarketBotDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MarketBotDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MarketBotDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [MarketBotDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MarketBotDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MarketBotDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MarketBotDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MarketBotDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MarketBotDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MarketBotDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MarketBotDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MarketBotDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MarketBotDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MarketBotDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MarketBotDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MarketBotDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MarketBotDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MarketBotDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MarketBotDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MarketBotDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MarketBotDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MarketBotDb] SET  MULTI_USER 
GO
ALTER DATABASE [MarketBotDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MarketBotDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MarketBotDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MarketBotDb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [MarketBotDb] SET DELAYED_DURABILITY = DISABLED 
GO
USE [MarketBotDb]
GO
/****** Object:  User [bot]    Script Date: 03.12.2017 21:49:06 ******/
CREATE USER [bot] FOR LOGIN [bot] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HouseId] [int] NULL,
	[FollowerId] [int] NULL,
 CONSTRAINT [PK_Adress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Admin]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FollowerId] [int] NULL,
	[DateAdd] [datetime] NULL,
	[AdminKeyId] [int] NULL,
	[Enable] [bit] NULL,
	[NotyfiActive] [bit] NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdminKey]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdminKey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyValue] [varchar](256) NULL,
	[DateAdd] [datetime] NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_AdminKey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentFs]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentFs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GuId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Fs] [varbinary](max) FILESTREAM  NULL,
	[Caption] [varchar](200) NULL,
	[AttachmentTypeId] [int] NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_AttachmentFs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] FILESTREAM_ON [BotDbFs],
 CONSTRAINT [UQ__Attachme__3214EC06D860A4AD] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Attachme__A2B66B0514E78B0A] UNIQUE NONCLUSTERED 
(
	[GuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] FILESTREAM_ON [BotDbFs]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentTelegram]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentTelegram](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileId] [varchar](100) NULL,
	[AttachmentFsId] [int] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttachmentType]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachmentType](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_AttachmentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Basket]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Basket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FollowerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Amount] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Enable] [bit] NOT NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_Cart_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BlackList]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlackList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAdd] [datetime] NOT NULL,
	[Duration] [int] NOT NULL,
	[FollowerId] [int] NOT NULL,
	[Deleted] [bit] NULL,
 CONSTRAINT [PK_Blacklist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BotInfo]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BotInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[ChatId] [int] NULL,
	[Token] [varchar](500) NULL,
	[Timestamp] [datetime] NULL,
 CONSTRAINT [PK_BotInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Category]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[City]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[RegionId] [int] NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Company]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[Text] [varchar](1000) NULL,
	[City] [varchar](100) NULL,
	[Vk] [varchar](100) NULL,
	[Instagram] [varchar](100) NULL,
	[Chanel] [varchar](100) NULL,
	[Telephone] [varchar](20) NULL,
	[Chat] [varchar](100) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Configuration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExampleCsvFileId] [varchar](100) NULL,
	[TemplateCsvFileId] [varchar](100) NULL,
	[BotBlocked] [bit] NULL,
	[ManualFileId] [varchar](100) NULL,
	[TemplateCsvFileIMD5] [varchar](500) NULL,
	[ExampleCsvFileMD5] [varchar](500) NULL,
	[ManualFileMD5] [varchar](500) NULL,
	[OwnerChatId] [int] NULL,
	[Telephone] [varchar](20) NULL,
	[PrivateGroupChatId] [varchar](50) NULL,
	[BotInfoId] [int] NULL,
	[VerifyTelephone] [bit] NULL,
	[OwnerPrivateNotify] [bit] NULL,
	[UserNameFaqFileId] [varchar](100) NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortName] [varchar](10) NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FeedBack]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FeedBack](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](500) NULL,
	[DateAdd] [datetime] NULL,
	[OrderId] [int] NULL,
	[RaitingId] [int] NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Follower]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Follower](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FIrstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[UserName] [varchar](500) NULL,
	[ChatType] [int] NULL,
	[Blocked] [bit] NULL,
	[Telephone] [varchar](50) NULL,
	[ChatId] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_followers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDesk]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HelpDesk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[Text] [varchar](1000) NULL,
	[Send] [bit] NULL,
	[Closed] [bit] NULL,
	[InWork] [bit] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_HelpDesk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDeskAnswer]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HelpDeskAnswer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HelpDeskId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[Text] [varchar](1000) NULL,
	[Closed] [bit] NULL,
	[ClosedTimestamp] [datetime] NULL,
 CONSTRAINT [PK_HelpDeskAnswer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HelpDeskAnswerAttachment]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskAnswerAttachment](
	[HelpDeskAnswerId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_HelpDeskAnswerAttachment] PRIMARY KEY CLUSTERED 
(
	[HelpDeskAnswerId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HelpDeskAttachment]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskAttachment](
	[HelpDeskId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_HelpDeskAttachment] PRIMARY KEY CLUSTERED 
(
	[HelpDeskId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HelpDeskInWork]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HelpDeskInWork](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HelpDeskId] [int] NULL,
	[FollowerId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[InWork] [bit] NULL,
 CONSTRAINT [PK_HelpDeskInWork] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[House]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[House](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [varchar](10) NULL,
	[StreetId] [int] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[ZipCode] [int] NULL,
	[Apartment] [int] NULL,
 CONSTRAINT [PK_House] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[FollowerId] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderAddress]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderAddress](
	[AdressId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[ShipPriceId] [int] NULL,
 CONSTRAINT [PK_OrderAdress] PRIMARY KEY CLUSTERED 
(
	[AdressId] ASC,
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderConfirm]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderConfirm](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAdd] [datetime] NULL,
	[Text] [varchar](500) NULL,
	[FollowerId] [int] NULL,
	[OrderId] [int] NULL,
	[Confirmed] [bit] NULL,
 CONSTRAINT [PK_OrderConfirm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderDeleted]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderDeleted](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[FollowerId] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Deleted] [bit] NULL,
	[Text] [varchar](500) NULL,
 CONSTRAINT [PK_OrderDeleted] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderDone]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDone](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[DateAdd] [datetime] NULL,
	[FollowerId] [int] NULL,
	[Done] [bit] NULL,
 CONSTRAINT [PK_DoneOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderPayment]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderPayment](
	[PaymentId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
 CONSTRAINT [PK_OrderPayment] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC,
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderProduct]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NULL,
	[Count] [int] NULL,
	[PriceId] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_OrderStr] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [decimal](18, 0) NULL,
	[FollowerId] [int] NOT NULL,
	[Text] [varchar](500) NULL,
	[DateAdd] [datetime] NULL,
	[Paid] [bit] NULL,
	[PaymentTypeId] [int] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrdersInWork]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdersInWork](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[Timestamp] [datetime] NULL,
	[FollowerId] [int] NULL,
	[InWork] [bit] NULL,
 CONSTRAINT [PK_OrdersInWork] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderTemp]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](500) NULL,
	[AddressId] [int] NULL,
	[FollowerId] [int] NULL,
	[PaymentTypeId] [int] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_OrderTemp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentTypeId] [int] NULL,
	[TxId] [varchar](200) NULL,
	[DataAdd] [datetime] NULL,
	[Comment] [varchar](500) NULL,
	[Summ] [float] NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Product]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Text] [varchar](500) NULL,
	[PhotoId] [varchar](500) NULL,
	[CategoryId] [int] NOT NULL,
	[Enable] [bit] NULL,
	[TelegraphUrl] [varchar](200) NULL,
	[DateAdd] [datetime] NULL,
	[PhotoUrl] [varchar](500) NULL,
	[UnitId] [int] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductPhoto]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPhoto](
	[ProductId] [int] NOT NULL,
	[AttachmentFsId] [int] NOT NULL,
 CONSTRAINT [PK_ProductPhoto] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[AttachmentFsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductPrice]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [float] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[DateAdd] [datetime] NULL,
	[Volume] [int] NULL,
	[CurrencyId] [int] NULL,
 CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QiwiApi]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QiwiApi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAdd] [datetime] NULL,
	[Token] [varchar](100) NULL,
	[Enable] [bit] NULL,
	[Telephone] [varchar](20) NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_QiwiApi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Raiting]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Raiting](
	[Id] [int] NOT NULL,
	[Value] [smallint] NULL,
 CONSTRAINT [PK_Raiting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Region]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Region](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportsRequestLog]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportsRequestLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAdd] [datetime] NULL,
	[FollowerId] [int] NULL,
 CONSTRAINT [PK_ReportsRequestLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShipPrice]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipPrice](
	[ShipPrice_id] [int] IDENTITY(1,1) NOT NULL,
	[ShipPrice_value] [float] NULL,
	[ShipPrice_enable] [bit] NULL,
 CONSTRAINT [PK_ShipPrice] PRIMARY KEY CLUSTERED 
(
	[ShipPrice_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stock]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Stock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Balance] [int] NULL,
	[DateAdd] [datetime] NULL,
	[Text] [varchar](500) NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Street]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Street](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[CityId] [int] NULL,
 CONSTRAINT [PK_Street] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TelegramMessage]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TelegramMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UpdateId] [int] NULL,
	[MessageId] [varchar](200) NULL,
	[FollowerId] [int] NULL,
	[UpdateJson] [varchar](max) NULL,
	[DateAdd] [datetime] NULL,
	[BotInfoId] [int] NULL,
 CONSTRAINT [PK_TelegramMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Units]    Script Date: 03.12.2017 21:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortName] [varchar](5) NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (1, N'Photo')
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (2, N'Video')
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (3, N'Audio')
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (4, N'Voice')
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (5, N'VideoNone')
INSERT [dbo].[AttachmentType] ([Id], [Name]) VALUES (6, N'Document')
INSERT [dbo].[Currency] ([Id], [Name], [ShortName]) VALUES (1, N'Рубли', N'руб.')
INSERT [dbo].[PaymentType] ([Id], [Name], [Enable]) VALUES (1, N'Наличными при получении', 1)
INSERT [dbo].[PaymentType] ([Id], [Name], [Enable]) VALUES (2, N'QIWI', 1)
INSERT [dbo].[Raiting] ([Id], [Value]) VALUES (1, 1)
INSERT [dbo].[Raiting] ([Id], [Value]) VALUES (2, 2)
INSERT [dbo].[Raiting] ([Id], [Value]) VALUES (3, 3)
INSERT [dbo].[Raiting] ([Id], [Value]) VALUES (4, 4)
INSERT [dbo].[Raiting] ([Id], [Value]) VALUES (5, 5)
SET IDENTITY_INSERT [dbo].[Units] ON 

INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (1, N'Штуки', N'шт.')
INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (2, N'Граммы', N'г.')
INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (3, N'Килограммы', N'кг.')
INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (4, N'Литры', N'л.')
INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (5, N'Метр', N'м.')
INSERT [dbo].[Units] ([Id], [Name], [ShortName]) VALUES (6, N'Сантиметр', N'см.')
SET IDENTITY_INSERT [dbo].[Units] OFF
/****** Object:  Index [IX_Basket_bot]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Basket_bot] ON [dbo].[Basket]
(
	[BotInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Basket_Follower]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Basket_Follower] ON [dbo].[Basket]
(
	[FollowerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Basket_Prod]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Basket_Prod] ON [dbo].[Basket]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Feedback]    Script Date: 03.12.2017 21:49:06 ******/
ALTER TABLE [dbo].[FeedBack] ADD  CONSTRAINT [IX_Feedback] UNIQUE NONCLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HelpDesk_Bot]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_HelpDesk_Bot] ON [dbo].[HelpDesk]
(
	[BotInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HelpDesk_Follower]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_HelpDesk_Follower] ON [dbo].[HelpDesk]
(
	[FollowerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderProduct_Order]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_OrderProduct_Order] ON [dbo].[OrderProduct]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderProduct_Prod]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_OrderProduct_Prod] ON [dbo].[OrderProduct]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_Bot]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_Bot] ON [dbo].[Orders]
(
	[BotInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_Follower]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_Follower] ON [dbo].[Orders]
(
	[FollowerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProductPhoto]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_ProductPhoto] ON [dbo].[ProductPhoto]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Stock_Prod]    Script Date: 03.12.2017 21:49:06 ******/
CREATE NONCLUSTERED INDEX [IX_Stock_Prod] ON [dbo].[Stock]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_House] FOREIGN KEY([HouseId])
REFERENCES [dbo].[House] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_House]
GO
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Adress_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Adress_Follower]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_AdminKey] FOREIGN KEY([AdminKeyId])
REFERENCES [dbo].[AdminKey] ([Id])
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_AdminKey]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_Follower]
GO
ALTER TABLE [dbo].[AttachmentFs]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentFs_AttachmentType] FOREIGN KEY([AttachmentTypeId])
REFERENCES [dbo].[AttachmentType] ([Id])
GO
ALTER TABLE [dbo].[AttachmentFs] CHECK CONSTRAINT [FK_AttachmentFs_AttachmentType]
GO
ALTER TABLE [dbo].[AttachmentTelegram]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_AttachmentFs] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[AttachmentTelegram] CHECK CONSTRAINT [FK_Attachment_AttachmentFs]
GO
ALTER TABLE [dbo].[AttachmentTelegram]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[AttachmentTelegram] CHECK CONSTRAINT [FK_Attachment_BotInfo]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Basket_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Basket_BotInfo]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Follower1] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Cart_Follower1]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Cart_Product]
GO
ALTER TABLE [dbo].[BlackList]  WITH CHECK ADD  CONSTRAINT [FK_Blacklist_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[BlackList] CHECK CONSTRAINT [FK_Blacklist_Follower]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_Region]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuration_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuration_BotInfo]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_Feedback_Orders]
GO
ALTER TABLE [dbo].[FeedBack]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Raiting] FOREIGN KEY([RaitingId])
REFERENCES [dbo].[Raiting] ([Id])
GO
ALTER TABLE [dbo].[FeedBack] CHECK CONSTRAINT [FK_Feedback_Raiting]
GO
ALTER TABLE [dbo].[HelpDesk]  WITH CHECK ADD  CONSTRAINT [FK_HelpDesk_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[HelpDesk] CHECK CONSTRAINT [FK_HelpDesk_BotInfo]
GO
ALTER TABLE [dbo].[HelpDesk]  WITH CHECK ADD  CONSTRAINT [FK_HelpDesk_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDesk] CHECK CONSTRAINT [FK_HelpDesk_Follower]
GO
ALTER TABLE [dbo].[HelpDeskAnswer]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswer_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswer] CHECK CONSTRAINT [FK_HelpDeskAnswer_Follower]
GO
ALTER TABLE [dbo].[HelpDeskAnswer]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswer_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswer] CHECK CONSTRAINT [FK_HelpDeskAnswer_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswerAttachment_Attachment] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment] CHECK CONSTRAINT [FK_HelpDeskAnswerAttachment_Attachment]
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAnswerAttachment_HelpDesk] FOREIGN KEY([HelpDeskAnswerId])
REFERENCES [dbo].[HelpDeskAnswer] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAnswerAttachment] CHECK CONSTRAINT [FK_HelpDeskAnswerAttachment_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAttachment_Attachment] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAttachment] CHECK CONSTRAINT [FK_HelpDeskAttachment_Attachment]
GO
ALTER TABLE [dbo].[HelpDeskAttachment]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskAttachment_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskAttachment] CHECK CONSTRAINT [FK_HelpDeskAttachment_HelpDesk]
GO
ALTER TABLE [dbo].[HelpDeskInWork]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskInWork_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskInWork] CHECK CONSTRAINT [FK_HelpDeskInWork_Follower]
GO
ALTER TABLE [dbo].[HelpDeskInWork]  WITH CHECK ADD  CONSTRAINT [FK_HelpDeskInWork_HelpDesk] FOREIGN KEY([HelpDeskId])
REFERENCES [dbo].[HelpDesk] ([Id])
GO
ALTER TABLE [dbo].[HelpDeskInWork] CHECK CONSTRAINT [FK_HelpDeskInWork_HelpDesk]
GO
ALTER TABLE [dbo].[House]  WITH CHECK ADD  CONSTRAINT [FK_House_Street] FOREIGN KEY([StreetId])
REFERENCES [dbo].[Street] ([Id])
GO
ALTER TABLE [dbo].[House] CHECK CONSTRAINT [FK_House_Street]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Follower]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Product]
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD  CONSTRAINT [FK_OrderAdress_Adress] FOREIGN KEY([AdressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OrderAddress] CHECK CONSTRAINT [FK_OrderAdress_Adress]
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD  CONSTRAINT [FK_OrderAdress_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderAddress] CHECK CONSTRAINT [FK_OrderAdress_Order]
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD  CONSTRAINT [FK_OrdersAdress_ShipPrice] FOREIGN KEY([ShipPriceId])
REFERENCES [dbo].[ShipPrice] ([ShipPrice_id])
GO
ALTER TABLE [dbo].[OrderAddress] CHECK CONSTRAINT [FK_OrdersAdress_ShipPrice]
GO
ALTER TABLE [dbo].[OrderConfirm]  WITH CHECK ADD  CONSTRAINT [FK_OrderConfirm_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrderConfirm] CHECK CONSTRAINT [FK_OrderConfirm_Follower]
GO
ALTER TABLE [dbo].[OrderConfirm]  WITH CHECK ADD  CONSTRAINT [FK_OrderConfirm_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderConfirm] CHECK CONSTRAINT [FK_OrderConfirm_Orders]
GO
ALTER TABLE [dbo].[OrderDeleted]  WITH CHECK ADD  CONSTRAINT [FK_OrderDeleted_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrderDeleted] CHECK CONSTRAINT [FK_OrderDeleted_Follower]
GO
ALTER TABLE [dbo].[OrderDeleted]  WITH CHECK ADD  CONSTRAINT [FK_OrderDeleted_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDeleted] CHECK CONSTRAINT [FK_OrderDeleted_Orders]
GO
ALTER TABLE [dbo].[OrderDone]  WITH CHECK ADD  CONSTRAINT [FK_OrderDone_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrderDone] CHECK CONSTRAINT [FK_OrderDone_Follower]
GO
ALTER TABLE [dbo].[OrderDone]  WITH CHECK ADD  CONSTRAINT [FK_OrderDone_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDone] CHECK CONSTRAINT [FK_OrderDone_Orders]
GO
ALTER TABLE [dbo].[OrderPayment]  WITH CHECK ADD  CONSTRAINT [FK_OrderPayment_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderPayment] CHECK CONSTRAINT [FK_OrderPayment_Orders]
GO
ALTER TABLE [dbo].[OrderPayment]  WITH CHECK ADD  CONSTRAINT [FK_OrderPayment_Payment] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payment] ([Id])
GO
ALTER TABLE [dbo].[OrderPayment] CHECK CONSTRAINT [FK_OrderPayment_Payment]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrdersStr_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrdersStr_Product]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderStr_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderStr_Order]
GO
ALTER TABLE [dbo].[OrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_OrderStr_Price] FOREIGN KEY([PriceId])
REFERENCES [dbo].[ProductPrice] ([Id])
GO
ALTER TABLE [dbo].[OrderProduct] CHECK CONSTRAINT [FK_OrderStr_Price]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Order_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Order_Follower]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_BotInfo]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_PaymentType]
GO
ALTER TABLE [dbo].[OrdersInWork]  WITH CHECK ADD  CONSTRAINT [FK_OrdersInWork_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrdersInWork] CHECK CONSTRAINT [FK_OrdersInWork_Follower]
GO
ALTER TABLE [dbo].[OrdersInWork]  WITH CHECK ADD  CONSTRAINT [FK_OrdersInWork_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrdersInWork] CHECK CONSTRAINT [FK_OrdersInWork_Orders]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_Address]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_BotInfo]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_Follower]
GO
ALTER TABLE [dbo].[OrderTemp]  WITH CHECK ADD  CONSTRAINT [FK_OrderTemp_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[OrderTemp] CHECK CONSTRAINT [FK_OrderTemp_PaymentType]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_PaymentType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Units] FOREIGN KEY([UnitId])
REFERENCES [dbo].[Units] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Units]
GO
ALTER TABLE [dbo].[ProductPhoto]  WITH CHECK ADD  CONSTRAINT [FK_ProductPhoto_AttachmentFs] FOREIGN KEY([AttachmentFsId])
REFERENCES [dbo].[AttachmentFs] ([Id])
GO
ALTER TABLE [dbo].[ProductPhoto] CHECK CONSTRAINT [FK_ProductPhoto_AttachmentFs]
GO
ALTER TABLE [dbo].[ProductPhoto]  WITH CHECK ADD  CONSTRAINT [FK_ProductPhoto_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPhoto] CHECK CONSTRAINT [FK_ProductPhoto_Product]
GO
ALTER TABLE [dbo].[ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_Price_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPrice] CHECK CONSTRAINT [FK_Price_Product]
GO
ALTER TABLE [dbo].[ProductPrice]  WITH CHECK ADD  CONSTRAINT [FK_ProductPrice_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[ProductPrice] CHECK CONSTRAINT [FK_ProductPrice_Currency]
GO
ALTER TABLE [dbo].[QiwiApi]  WITH CHECK ADD  CONSTRAINT [FK_QiwiApi_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[QiwiApi] CHECK CONSTRAINT [FK_QiwiApi_BotInfo]
GO
ALTER TABLE [dbo].[ReportsRequestLog]  WITH CHECK ADD  CONSTRAINT [FK_ReportsRequestLog_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[ReportsRequestLog] CHECK CONSTRAINT [FK_ReportsRequestLog_Follower]
GO
ALTER TABLE [dbo].[Stock]  WITH CHECK ADD  CONSTRAINT [FK_Stock_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Stock] CHECK CONSTRAINT [FK_Stock_Product]
GO
ALTER TABLE [dbo].[Street]  WITH CHECK ADD  CONSTRAINT [FK_Street_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Street] CHECK CONSTRAINT [FK_Street_City]
GO
ALTER TABLE [dbo].[TelegramMessage]  WITH CHECK ADD  CONSTRAINT [FK_TelegramMessage_BotInfo] FOREIGN KEY([BotInfoId])
REFERENCES [dbo].[BotInfo] ([Id])
GO
ALTER TABLE [dbo].[TelegramMessage] CHECK CONSTRAINT [FK_TelegramMessage_BotInfo]
GO
ALTER TABLE [dbo].[TelegramMessage]  WITH CHECK ADD  CONSTRAINT [FK_TelegramMessage_Follower] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Follower] ([Id])
GO
ALTER TABLE [dbo].[TelegramMessage] CHECK CONSTRAINT [FK_TelegramMessage_Follower]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Еденицы измерения' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Units'
GO
USE [master]
GO
ALTER DATABASE [MarketBotDb] SET  READ_WRITE 
GO
