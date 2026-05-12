-- ============================================================
-- SHADI HALL DATABASE — Initial Migration Script
-- Run this AFTER: dotnet ef migrations add InitialCreate
--                 dotnet ef database update
-- OR use this script directly in SSMS on ShadiHallDb
-- ============================================================

-- This file is for reference. EF Core will auto-generate migrations.
-- Use Package Manager Console in Visual Studio:
--   Add-Migration InitialCreate
--   Update-Database

-- Manual creation if needed:
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ShadiHallDb')
BEGIN
    CREATE DATABASE ShadiHallDb;
END
GO

USE ShadiHallDb;
GO

-- After running EF migrations the following tables will be created:
--   dbo.Users          (AspNetUsers extended with FullName, Address, LastLogin)
--   dbo.Roles          (Admin, User roles)
--   dbo.UserRoles
--   dbo.UserClaims
--   dbo.UserLogins
--   dbo.UserTokens
--   dbo.RoleClaims
--   dbo.Halls
--   dbo.Reservations
--   dbo.Reviews
--   dbo.Payments
--   dbo.ContactMessages
--   dbo.BannerSlides

-- Default admin credentials (seeded by ApplicationDbContext):
--   Email:    admin@shadihall.com
--   Password: Admin@123!
--   Role:     Admin

PRINT 'ShadiHallDb is ready. Run EF migrations to create tables.';
GO
