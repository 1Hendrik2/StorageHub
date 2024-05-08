USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StorageHub')
BEGIN
    CREATE DATABASE StorageHub;
END