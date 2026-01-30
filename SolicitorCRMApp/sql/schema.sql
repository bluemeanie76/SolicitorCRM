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

CREATE TABLE dbo.Pools (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    Enabled BIT NOT NULL DEFAULT 1,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateUpdated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.UserPools (
    UserId INT NOT NULL,
    PoolId INT NOT NULL,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_UserPools PRIMARY KEY (UserId, PoolId),
    CONSTRAINT FK_UserPools_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT FK_UserPools_Pools FOREIGN KEY (PoolId) REFERENCES dbo.Pools(Id)
);

CREATE TABLE dbo.Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IsUrgent BIT NOT NULL DEFAULT 0,
    ContactName NVARCHAR(200) NOT NULL,
    ContactEmail NVARCHAR(255) NOT NULL,
    ContactTelephone NVARCHAR(50) NULL,
    ContactNotes NVARCHAR(1000) NULL,
    TaskDescription NVARCHAR(2000) NOT NULL,
    TaskDeadline DATETIME2 NOT NULL,
    AssignedUserId INT NULL,
    AssignedPoolId INT NULL,
    CreatedByUserId INT NOT NULL,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DateUpdated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Tasks_AssignedUser FOREIGN KEY (AssignedUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Tasks_AssignedPool FOREIGN KEY (AssignedPoolId) REFERENCES dbo.Pools(Id),
    CONSTRAINT FK_Tasks_CreatedByUser FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_Tasks_Assignment CHECK (AssignedUserId IS NOT NULL OR AssignedPoolId IS NOT NULL)
);

CREATE TABLE dbo.TaskNotes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TaskId INT NOT NULL,
    Note NVARCHAR(2000) NOT NULL,
    CreatedByUserId INT NOT NULL,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_TaskNotes_Tasks FOREIGN KEY (TaskId) REFERENCES dbo.Tasks(Id),
    CONSTRAINT FK_TaskNotes_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);

CREATE TABLE dbo.TaskTimeEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TaskId INT NOT NULL,
    LoggedByUserId INT NOT NULL,
    Hours INT NOT NULL,
    Minutes INT NOT NULL,
    TotalMinutes AS (Hours * 60 + Minutes) PERSISTED,
    DateAdded DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_TaskTimeEntries_Tasks FOREIGN KEY (TaskId) REFERENCES dbo.Tasks(Id),
    CONSTRAINT FK_TaskTimeEntries_Users FOREIGN KEY (LoggedByUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_TaskTimeEntries_Hours CHECK (Hours >= 0),
    CONSTRAINT CK_TaskTimeEntries_Minutes CHECK (Minutes BETWEEN 0 AND 59)
);
GO

INSERT INTO dbo.UserTypes (Name)
VALUES ('super-administrator'), ('administrator'), ('user');
GO

INSERT INTO dbo.Users (Title, FirstName, Surname, Email, PasswordHash, PasswordSalt, JobTitle, Department, UserTypeId, Enabled)
VALUES ('Ms', 'Admin', 'User', 'admin@solicitorcrm.local',
        'EMrU2+MundKQ7GBs8Zxd8ogzKdvIlX5H8bK+6uOg+MU=',
        'hbsP+st3j3XNag53uVu1IA==',
        'System Administrator', 'IT', 1, 1);
GO
