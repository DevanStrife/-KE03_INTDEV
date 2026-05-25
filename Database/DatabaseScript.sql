-- Matrix Inc Database Creation Script
-- Dit script maakt de database aan en vult deze met seed data

USE master;
GO

-- Maak database aan als deze niet bestaat
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MatrixIncDb')
BEGIN
    CREATE DATABASE MatrixIncDb;
END
GO

USE MatrixIncDb;
GO

-- Tables worden automatisch aangemaakt door Entity Framework Migrations
-- Bij het starten van de applicatie worden de tabellen en seed data automatisch aangemaakt
