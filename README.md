# SolicitorCRM

## Overview
This repository contains a simple ASP.NET Core MVC application that provides:
- A secure login page
- A landing page for authenticated users
- A user administration screen restricted to administrators

## Database setup
1. Create the database and tables:
   - Run `sql/schema.sql` in SQL Server.
2. Create stored procedures:
   - Run `sql/stored-procedures.sql`.

A default administrator account is seeded:
- **Email:** `admin@solicitorcrm.local`
- **Password:** `ChangeMe123!`

Update the connection string in `SolicitorCRMApp/appsettings.json` to match your SQL Server instance.

## Application
The app entry point is in `SolicitorCRMApp/Program.cs`.
