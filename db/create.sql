USE [master]
GO

DECLARE @Created bit
SET @Created = 0

IF NOT EXISTS(SELECT * FROM sys.databases WHERE [name] = '{{DatabaseName}}')
BEGIN
	SET @Created = 1

	CREATE DATABASE [{{DatabaseName}}]

	ALTER DATABASE [{{DatabaseName}}] SET COMPATIBILITY_LEVEL = 130
END

SELECT @Created
GO
