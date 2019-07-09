USE [master]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Sales')
DROP DATABASE [Sales]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Warehouse')
DROP DATABASE [Warehouse]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Shipping')
DROP DATABASE [Shipping]
GO

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Marketing')
DROP DATABASE [Marketing]
GO
