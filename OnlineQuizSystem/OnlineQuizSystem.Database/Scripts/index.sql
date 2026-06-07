-- Ensure filtered unique index on active quiz sessions (one active session per user per teacher)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes i
    WHERE i.name = 'UX_QuizSessions_UserID_TeacherID_Active'
      AND i.object_id = OBJECT_ID('dbo.QuizSessions')
)
BEGIN
    CREATE UNIQUE INDEX UX_QuizSessions_UserID_TeacherID_Active
    ON dbo.QuizSessions (UserID, TeacherID)
    WHERE IsSubmitted = 0 AND TeacherID IS NOT NULL;
END;
GO

-- Ensure unique index on Results to prevent duplicate answers for the same question in a session
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes i
    WHERE i.name = 'UX_Results_Session_Question'
      AND i.object_id = OBJECT_ID('dbo.Results')
)
BEGIN
    CREATE UNIQUE INDEX UX_Results_Session_Question
    ON dbo.Results (SessionID, QuestionID);
END;
GO
