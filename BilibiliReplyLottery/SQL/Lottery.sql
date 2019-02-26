--*******创建数据库*******--
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'LotteryData')  
DROP DATABASE LotteryData
GO
CREATE DATABASE LotteryData
GO

USE LotteryData

GO

IF EXISTS(SELECT * FROM SYSOBJECTS WHERE name = 'LotteryReady')
DROP TABLE LotteryReady
GO
CREATE TABLE LotteryReady
(
	LotteryId INT PRIMARY KEY IDENTITY(1,1),
	AvNum NVARCHAR(50),
	LotteryNum INT,
	LotteryTime Datetime,
	IsFilter INT,
	IsExecuted INT,
)
GO

IF EXISTS(SELECT * FROM SYSOBJECTS WHERE name = 'LotteryResult')
DROP TABLE LotteryResult
GO
CREATE TABLE LotteryResult
(
	ResultID INT PRIMARY KEY IDENTITY(1,1),
	LotteryId INT,
	Name NVARCHAR(100),
	mid NVARCHAR(30),
	LotteryFloor INT,
	Msg NVARCHAR(200),
	LotteryTime datetime,
)
GO

IF EXISTS(SELECT * FROM SYSOBJECTS WHERE name = 'LotteryAccount')
DROP TABLE LotteryAccount
GO
CREATE TABLE LotteryAccount
(
	LID INT PRIMARY KEY IDENTITY(1,1),
	LotteryName NVARCHAR(50),
	LotteryPsw NVARCHAR(50),
	LotteryCount INT
)
GO
