-- Procedure to get student information.
CREATE PROCEDURE dbo.sp_GetStudentInfo
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM Users 
    WHERE UserID = @UserID;
END;
GO
-- Procedure to get teacher information.
CREATE PROCEDURE dbo.sp_GetTeacherInfo
    @TeacherID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM Teachers
    WHERE TeacherID = @TeacherID;
END;
GO