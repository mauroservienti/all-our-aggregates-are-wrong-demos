USE [master]
GO

:setvar DatabaseName "Sales"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = '$(DatabaseName)', FILENAME = '$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Warehouse"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = '$(DatabaseName)', FILENAME = '$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Shipping"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = '$(DatabaseName)', FILENAME = '$(UserPath)\$(DatabaseName).mdf' )
GO

:setvar DatabaseName "Marketing"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = '$(DatabaseName)', FILENAME = '$(UserPath)\$(DatabaseName).mdf' )
GO
