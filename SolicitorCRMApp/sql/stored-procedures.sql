USE SolicitorCRM;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
           u.Title,
           u.FirstName,
           u.Surname,
           u.Email,
           u.PasswordHash,
           u.PasswordSalt,
           u.JobTitle,
           u.Department,
           u.UserTypeId,
           ut.Name AS UserTypeName,
           u.Enabled,
           u.DateAdded,
           u.DateUpdated
    FROM dbo.Users u
    INNER JOIN dbo.UserTypes ut ON u.UserTypeId = ut.Id
    ORDER BY u.Surname, u.FirstName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_GetByEmail
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
           u.Title,
           u.FirstName,
           u.Surname,
           u.Email,
           u.PasswordHash,
           u.PasswordSalt,
           u.JobTitle,
           u.Department,
           u.UserTypeId,
           ut.Name AS UserTypeName,
           u.Enabled,
           u.DateAdded,
           u.DateUpdated
    FROM dbo.Users u
    INNER JOIN dbo.UserTypes ut ON u.UserTypeId = ut.Id
    WHERE u.Email = @Email;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_GetById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
           u.Title,
           u.FirstName,
           u.Surname,
           u.Email,
           u.PasswordHash,
           u.PasswordSalt,
           u.JobTitle,
           u.Department,
           u.UserTypeId,
           ut.Name AS UserTypeName,
           u.Enabled,
           u.DateAdded,
           u.DateUpdated
    FROM dbo.Users u
    INNER JOIN dbo.UserTypes ut ON u.UserTypeId = ut.Id
    WHERE u.Id = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_Insert
    @Title NVARCHAR(20),
    @FirstName NVARCHAR(100),
    @Surname NVARCHAR(100),
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(256),
    @PasswordSalt NVARCHAR(256),
    @JobTitle NVARCHAR(100),
    @Department NVARCHAR(100),
    @UserTypeId INT,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (Title, FirstName, Surname, Email, PasswordHash, PasswordSalt, JobTitle, Department, UserTypeId, Enabled)
    VALUES (@Title, @FirstName, @Surname, @Email, @PasswordHash, @PasswordSalt, @JobTitle, @Department, @UserTypeId, @Enabled);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_Update
    @UserId INT,
    @Title NVARCHAR(20),
    @FirstName NVARCHAR(100),
    @Surname NVARCHAR(100),
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(256) = NULL,
    @PasswordSalt NVARCHAR(256) = NULL,
    @JobTitle NVARCHAR(100),
    @Department NVARCHAR(100),
    @UserTypeId INT,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET Title = @Title,
        FirstName = @FirstName,
        Surname = @Surname,
        Email = @Email,
        PasswordHash = COALESCE(@PasswordHash, PasswordHash),
        PasswordSalt = COALESCE(@PasswordSalt, PasswordSalt),
        JobTitle = @JobTitle,
        Department = @Department,
        UserTypeId = @UserTypeId,
        Enabled = @Enabled,
        DateUpdated = SYSUTCDATETIME()
    WHERE Id = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_SetEnabled
    @UserId INT,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET Enabled = @Enabled,
        DateUpdated = SYSUTCDATETIME()
    WHERE Id = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_UserTypes_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM dbo.UserTypes
    ORDER BY Name;
END;
GO
