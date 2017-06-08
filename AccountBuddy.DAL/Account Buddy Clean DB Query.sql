delete from PaymentDetail
delete from Payment

delete from ReceiptDetail
delete from Receipt

delete from JournalDetail
delete from Journal

delete from Supplier
delete from Customer
delete from Bank

delete from Ledger

delete from Product
delete from UOM
delete from StockGroup
delete from AccountGroup

delete from CompanyDetail

delete from UserTypeDetail
delete from UserAccount
delete from UserType

delete from LogDetail
delete from LogMaster

delete from ErrorLog
USE [DBFMCG]
GO

/****** Object:  Table [dbo].[DataKeyValue]    Script Date: 06/06/2017 09:44:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DataKeyValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DataKey] [nvarchar](250) NOT NULL,
	[DataValue] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_DataKeyValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


