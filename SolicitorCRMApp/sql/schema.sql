CREATE DATABASE SolicitorCRM;
GO

USE SolicitorCRM;
GO

CREATE TABLE dbo.UserTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(20) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(256) NOT NULL,
    JobTitle NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    UserTypeId INT NOT NULL,
    Enabled BIT NOT NULL DEFAULT 1,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateUpdated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Users_UserTypes FOREIGN KEY (UserTypeId) REFERENCES dbo.UserTypes(Id)
);
GO

INSERT INTO dbo.UserTypes (Name)
VALUES ('administrator'), ('user');
GO

INSERT INTO dbo.Users (Title, FirstName, Surname, Email, PasswordHash, PasswordSalt, JobTitle, Department, UserTypeId, Enabled)
VALUES ('Ms', 'Admin', 'User', 'admin@solicitorcrm.local',
        'EMrU2+MundKQ7GBs8Zxd8ogzKdvIlX5H8bK+6uOg+MU=',
        'hbsP+st3j3XNag53uVu1IA==',
        'System Administrator', 'IT', 1, 1);
GO
