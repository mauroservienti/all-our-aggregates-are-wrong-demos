USE [master]
GO

:setvar DatabaseName "Sales"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Warehouse"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Shipping"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Marketing"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO
