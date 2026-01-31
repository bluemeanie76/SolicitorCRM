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

CREATE OR ALTER PROCEDURE dbo.usp_Pool_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name,
           Description,
           Enabled,
           DateAdded,
           DateUpdated
    FROM dbo.Pools
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_Insert
    @Name NVARCHAR(100),
    @Description NVARCHAR(255) = NULL,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Pools (Name, Description, Enabled)
    VALUES (@Name, @Description, @Enabled);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_Update
    @PoolId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(255) = NULL,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Pools
    SET Name = @Name,
        Description = @Description,
        Enabled = @Enabled,
        DateUpdated = SYSUTCDATETIME()
    WHERE Id = @PoolId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_SetEnabled
    @PoolId INT,
    @Enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Pools
    SET Enabled = @Enabled,
        DateUpdated = SYSUTCDATETIME()
    WHERE Id = @PoolId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_AssignUser
    @PoolId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.UserPools
        WHERE PoolId = @PoolId AND UserId = @UserId
    )
    BEGIN
        INSERT INTO dbo.UserPools (PoolId, UserId)
        VALUES (@PoolId, @UserId);
    END
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_RemoveUser
    @PoolId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UserPools
    WHERE PoolId = @PoolId AND UserId = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pool_GetUsers
    @PoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
           u.Title,
           u.FirstName,
           u.Surname,
           u.Email,
           u.JobTitle,
           u.Department,
           u.UserTypeId,
           ut.Name AS UserTypeName,
           u.Enabled,
           u.DateAdded,
           u.DateUpdated
    FROM dbo.Users u
    INNER JOIN dbo.UserTypes ut ON u.UserTypeId = ut.Id
    INNER JOIN dbo.UserPools up ON u.Id = up.UserId
    WHERE up.PoolId = @PoolId
    ORDER BY u.Surname, u.FirstName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_Insert
    @IsUrgent BIT,
    @ContactName NVARCHAR(200),
    @ContactEmail NVARCHAR(255),
    @ContactTelephone NVARCHAR(50) = NULL,
    @ContactNotes NVARCHAR(1000) = NULL,
    @TaskDescription NVARCHAR(2000),
    @TaskDeadline DATETIME2,
    @AssignedUserId INT = NULL,
    @AssignedPoolId INT = NULL,
    @CreatedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Tasks (IsUrgent, ContactName, ContactEmail, ContactTelephone, ContactNotes, TaskDescription,
                           TaskDeadline, AssignedUserId, AssignedPoolId, CreatedByUserId)
    VALUES (@IsUrgent, @ContactName, @ContactEmail, @ContactTelephone, @ContactNotes, @TaskDescription,
            @TaskDeadline, @AssignedUserId, @AssignedPoolId, @CreatedByUserId);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_Update
    @TaskId INT,
    @IsUrgent BIT,
    @ContactName NVARCHAR(200),
    @ContactEmail NVARCHAR(255),
    @ContactTelephone NVARCHAR(50) = NULL,
    @ContactNotes NVARCHAR(1000) = NULL,
    @TaskDescription NVARCHAR(2000),
    @TaskDeadline DATETIME2,
    @AssignedUserId INT = NULL,
    @AssignedPoolId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Tasks
    SET IsUrgent = @IsUrgent,
        ContactName = @ContactName,
        ContactEmail = @ContactEmail,
        ContactTelephone = @ContactTelephone,
        ContactNotes = @ContactNotes,
        TaskDescription = @TaskDescription,
        TaskDeadline = @TaskDeadline,
        AssignedUserId = @AssignedUserId,
        AssignedPoolId = @AssignedPoolId,
        DateUpdated = SYSUTCDATETIME()
    WHERE Id = @TaskId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetById
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id,
           t.IsUrgent,
           t.ContactName,
           t.ContactEmail,
           t.ContactTelephone,
           t.ContactNotes,
           t.TaskDescription,
           t.TaskDeadline,
           t.AssignedUserId,
           CONCAT(assignedUser.FirstName, ' ', assignedUser.Surname) AS AssignedUserName,
           t.AssignedPoolId,
           assignedPool.Name AS AssignedPoolName,
           t.CreatedByUserId,
           COALESCE(timeTotals.TotalMinutes, 0) AS TotalMinutes,
           t.DateAdded,
           t.DateUpdated
    FROM dbo.Tasks t
    LEFT JOIN dbo.Users assignedUser ON t.AssignedUserId = assignedUser.Id
    LEFT JOIN dbo.Pools assignedPool ON t.AssignedPoolId = assignedPool.Id
    OUTER APPLY (
        SELECT SUM(TotalMinutes) AS TotalMinutes
        FROM dbo.TaskTimeEntries
        WHERE TaskId = t.Id
    ) timeTotals
    WHERE t.Id = @TaskId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id,
           t.IsUrgent,
           t.ContactName,
           t.ContactEmail,
           t.ContactTelephone,
           t.ContactNotes,
           t.TaskDescription,
           t.TaskDeadline,
           t.AssignedUserId,
           CONCAT(assignedUser.FirstName, ' ', assignedUser.Surname) AS AssignedUserName,
           t.AssignedPoolId,
           assignedPool.Name AS AssignedPoolName,
           t.CreatedByUserId,
           COALESCE(timeTotals.TotalMinutes, 0) AS TotalMinutes,
           t.DateAdded,
           t.DateUpdated
    FROM dbo.Tasks t
    LEFT JOIN dbo.Users assignedUser ON t.AssignedUserId = assignedUser.Id
    LEFT JOIN dbo.Pools assignedPool ON t.AssignedPoolId = assignedPool.Id
    OUTER APPLY (
        SELECT SUM(TotalMinutes) AS TotalMinutes
        FROM dbo.TaskTimeEntries
        WHERE TaskId = t.Id
    ) timeTotals
    ORDER BY t.IsUrgent DESC, t.TaskDeadline ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetAssignedToUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id,
           t.IsUrgent,
           t.ContactName,
           t.ContactEmail,
           t.ContactTelephone,
           t.ContactNotes,
           t.TaskDescription,
           t.TaskDeadline,
           t.AssignedUserId,
           CONCAT(assignedUser.FirstName, ' ', assignedUser.Surname) AS AssignedUserName,
           t.AssignedPoolId,
           assignedPool.Name AS AssignedPoolName,
           t.CreatedByUserId,
           COALESCE(timeTotals.TotalMinutes, 0) AS TotalMinutes,
           t.DateAdded,
           t.DateUpdated
    FROM dbo.Tasks t
    LEFT JOIN dbo.Users assignedUser ON t.AssignedUserId = assignedUser.Id
    LEFT JOIN dbo.Pools assignedPool ON t.AssignedPoolId = assignedPool.Id
    OUTER APPLY (
        SELECT SUM(TotalMinutes) AS TotalMinutes
        FROM dbo.TaskTimeEntries
        WHERE TaskId = t.Id
    ) timeTotals
    WHERE t.AssignedUserId = @UserId
    ORDER BY t.IsUrgent DESC, t.TaskDeadline ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetAssignedToUserPools
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT t.Id,
           t.IsUrgent,
           t.ContactName,
           t.ContactEmail,
           t.ContactTelephone,
           t.ContactNotes,
           t.TaskDescription,
           t.TaskDeadline,
           t.AssignedUserId,
           CONCAT(assignedUser.FirstName, ' ', assignedUser.Surname) AS AssignedUserName,
           t.AssignedPoolId,
           assignedPool.Name AS AssignedPoolName,
           t.CreatedByUserId,
           COALESCE(timeTotals.TotalMinutes, 0) AS TotalMinutes,
           t.DateAdded,
           t.DateUpdated
    FROM dbo.Tasks t
    INNER JOIN dbo.UserPools up ON t.AssignedPoolId = up.PoolId
    LEFT JOIN dbo.Users assignedUser ON t.AssignedUserId = assignedUser.Id
    LEFT JOIN dbo.Pools assignedPool ON t.AssignedPoolId = assignedPool.Id
    OUTER APPLY (
        SELECT SUM(TotalMinutes) AS TotalMinutes
        FROM dbo.TaskTimeEntries
        WHERE TaskId = t.Id
    ) timeTotals
    WHERE up.UserId = @UserId
      AND t.AssignedUserId IS NULL
    ORDER BY t.IsUrgent DESC, t.TaskDeadline ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_AddNote
    @TaskId INT,
    @CreatedByUserId INT,
    @Note NVARCHAR(2000)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TaskNotes (TaskId, Note, CreatedByUserId)
    VALUES (@TaskId, @Note, @CreatedByUserId);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetNotes
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           TaskId,
           Note,
           CreatedByUserId,
           DateAdded
    FROM dbo.TaskNotes
    WHERE TaskId = @TaskId
    ORDER BY DateAdded DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_LogTime
    @TaskId INT,
    @LoggedByUserId INT,
    @Hours INT,
    @Minutes INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TaskTimeEntries (TaskId, LoggedByUserId, Hours, Minutes)
    VALUES (@TaskId, @LoggedByUserId, @Hours, @Minutes);

    SELECT SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Task_GetTimeEntries
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT te.Id,
           te.TaskId,
           te.LoggedByUserId,
           te.Hours,
           te.Minutes,
			te.TotalMinutes,
           te.DateAdded,
		   u.FirstName + ' ' + u.Surname as LoggedByName
    FROM [dbo].[TaskTimeEntries] te
	JOIN [dbo].[Users] u on te.LoggedByUserId = u.Id
    WHERE TaskId = @TaskId
    ORDER BY DateAdded DESC;
END;
GO
