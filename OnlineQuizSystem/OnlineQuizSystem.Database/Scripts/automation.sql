USE OnlineQuizDB;
GO

/*
  automation.sql
  -------------
  This script adds:
    1. Views for reporting and easy summaries
    2. Scalar functions for reusable business logic
    3. A stored procedure for recording quiz answers
    4. A trigger to keep session totals in sync automatically
    5. A seed automation procedure to insert sample data

  The final section calls the objects so you can see them working.
*/

-- Drop existing objects so the script can be rerun safely.
IF OBJECT_ID('dbo.vw_UserSessionSummary', 'V') IS NOT NULL
    DROP VIEW dbo.vw_UserSessionSummary;
GO
IF OBJECT_ID('dbo.vw_SectionEnrollmentDetails', 'V') IS NOT NULL
    DROP VIEW dbo.vw_SectionEnrollmentDetails;
GO
IF OBJECT_ID('dbo.fn_GetSessionPercent', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GetSessionPercent;
GO
IF OBJECT_ID('dbo.fn_IsAnswerCorrect', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_IsAnswerCorrect;
GO
IF OBJECT_ID('dbo.sp_RecordQuizAnswer', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RecordQuizAnswer;
GO
IF OBJECT_ID('dbo.sp_SeedSampleData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SeedSampleData;
GO
IF OBJECT_ID('dbo.trg_UpdateSessionTotalsOnResultInsert', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_UpdateSessionTotalsOnResultInsert;
GO

-- Function: convert raw score into a percentage safely.
CREATE FUNCTION dbo.fn_GetSessionPercent(
    @Score INT,
    @TotalQuestions INT
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @Percent DECIMAL(5,2);
    IF @TotalQuestions = 0
        SET @Percent = 0.00;
    ELSE
        SET @Percent = ROUND((CAST(@Score AS DECIMAL(10,2)) / @TotalQuestions) * 100.0, 2);

    RETURN @Percent;
END;
GO

-- Function: answer correctness helper.
CREATE FUNCTION dbo.fn_IsAnswerCorrect(
    @QuestionID INT,
    @SelectedAnswer CHAR(1)
)
RETURNS BIT
AS
BEGIN
    DECLARE @CorrectOption CHAR(1);

    SELECT @CorrectOption = CorrectOption
    FROM Questions
    WHERE QuestionID = @QuestionID;

    IF @CorrectOption IS NULL
        RETURN 0; -- unknown question means not correct

    RETURN CASE WHEN @SelectedAnswer = @CorrectOption THEN 1 ELSE 0 END;
END;
GO

-- View: session summary by user. Shows the quiz score and easy-to-read percentage.
CREATE VIEW dbo.vw_UserSessionSummary
AS
SELECT
    qs.SessionID,
    u.UserID,
    u.Name AS UserName,
    qs.Score,
    qs.TotalQuestions,
    dbo.fn_GetSessionPercent(qs.Score, qs.TotalQuestions) AS ScorePercent,
    qs.StartTime,
    qs.EndTime,
    qs.IsSubmitted
FROM QuizSessions qs
INNER JOIN Users u
    ON qs.UserID = u.UserID;
GO

-- View: section enrollment details. Shows how many students are enrolled in each section.
CREATE VIEW dbo.vw_SectionEnrollmentDetails
AS
SELECT
    s.SectionID,
    s.SectionName,
    t.Name AS TeacherName,
    COUNT(us.UserSectionID) AS EnrolledUserCount
FROM Sections s
INNER JOIN Teachers t
    ON s.TeacherID = t.TeacherID
LEFT JOIN UserSections us
    ON s.SectionID = us.SectionID
GROUP BY
    s.SectionID,
    s.SectionName,
    t.Name;
GO

-- Procedure: insert a quiz answer and let the trigger update session totals.
CREATE PROCEDURE dbo.sp_RecordQuizAnswer
    @SessionID INT,
    @UserID INT,
    @QuestionID INT,
    @SelectedAnswer CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate the session and user quickly.
    IF NOT EXISTS (SELECT 1 FROM QuizSessions WHERE SessionID = @SessionID AND UserID = @UserID)
    BEGIN
        RAISERROR('Session %d does not exist for user %d.', 16, 1, @SessionID, @UserID);
        RETURN;
    END;

    -- Insert the answer result. The trigger will update the session totals.
    INSERT INTO Results (UserID, QuestionID, SelectedAnswer, IsCorrect, SessionID)
    VALUES (
        @UserID,
        @QuestionID,
        @SelectedAnswer,
        dbo.fn_IsAnswerCorrect(@QuestionID, @SelectedAnswer),
        @SessionID
    );
END;
GO

-- Trigger: automatically update session score and question count after results are inserted.
CREATE TRIGGER dbo.trg_UpdateSessionTotalsOnResultInsert
ON dbo.Results
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE qs
    SET
        qs.TotalQuestions = qs.TotalQuestions + i.InsertedCount,
        qs.Score = qs.Score + i.CorrectCount
    FROM QuizSessions qs
    INNER JOIN (
        SELECT
            SessionID,
            COUNT(*) AS InsertedCount,
            SUM(CAST(IsCorrect AS INT)) AS CorrectCount
        FROM inserted
        GROUP BY SessionID
    ) AS i
        ON qs.SessionID = i.SessionID;
END;
GO

-- Automation: seed sample data if the tables are empty.
CREATE PROCEDURE dbo.sp_SeedSampleData
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert one teacher if needed.
    IF NOT EXISTS (SELECT 1 FROM Teachers)
    BEGIN
        INSERT INTO Teachers (Name, Email, Password, Role)
        VALUES ('Alice Teacher', 'alice.teacher@example.com', 'Password123', 'Teacher');
    END;

    -- Insert one admin if needed.
    IF NOT EXISTS (SELECT 1 FROM Admins)
    BEGIN
        INSERT INTO Admins (Name, Email, Password)
        VALUES ('Admin User', 'admin@example.com', 'AdminPass');
    END;

    -- Insert one user and assign a teacher.
    IF NOT EXISTS (SELECT 1 FROM Users)
    BEGIN
        INSERT INTO Users (Name, Email, Password, Role, TeacherID)
        VALUES ('Bob Student', 'bob.student@example.com', 'StudentPass', 'Student', (SELECT TOP 1 TeacherID FROM Teachers));
    END;

    -- Insert one section if needed.
    IF NOT EXISTS (SELECT 1 FROM Sections)
    BEGIN
        INSERT INTO Sections (SectionName, TeacherID, Description)
        VALUES ('Math Basics', (SELECT TOP 1 TeacherID FROM Teachers), 'A beginner section for math and quiz practice.');
    END;

    -- Enroll the user in the sample section.
    IF NOT EXISTS (SELECT 1 FROM UserSections)
    BEGIN
        INSERT INTO UserSections (UserID, SectionID)
        VALUES (
            (SELECT TOP 1 UserID FROM Users),
            (SELECT TOP 1 SectionID FROM Sections)
        );
    END;

    -- Add sample questions if none exist.
    IF NOT EXISTS (SELECT 1 FROM Questions)
    BEGIN
        INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID, SectionID)
        VALUES
            ('What is 2 + 2?', '3', '4', '5', '6', 'B', 1, (SELECT TOP 1 TeacherID FROM Teachers), (SELECT TOP 1 SectionID FROM Sections)),
            ('Which sentence is a question?', 'I like cake.', 'Do you like cake?', 'Cake is tasty.', 'We are eating cake.', 'B', 1, (SELECT TOP 1 TeacherID FROM Teachers), (SELECT TOP 1 SectionID FROM Sections));
    END;

    -- Create an initial quiz session for the sample user.
    IF NOT EXISTS (SELECT 1 FROM QuizSessions)
    BEGIN
        INSERT INTO QuizSessions (UserID, TeacherID, Score, StartTime, TotalQuestions, IsSubmitted)
        VALUES (
            (SELECT TOP 1 UserID FROM Users),
            (SELECT TOP 1 TeacherID FROM Teachers),
            0,
            GETDATE(),
            0,
            0
        );
    END;
END;
GO

/*
  Example calls below show how each object works.
  1) Seed data automatically.
  2) Record an answer and let the trigger update totals.
  3) Query the views and functions to verify the results.
  4) Inspect the inserted Results row and session totals.
*/

EXEC dbo.sp_SeedSampleData;
GO

-- Example: record an answer for a valid existing session and question.


-- Example: query the session summary view.
SELECT *
FROM dbo.vw_UserSessionSummary;
GO

-- Example: query the section enrollment detail view.
SELECT *
FROM dbo.vw_SectionEnrollmentDetails;
GO

-- Example: query the raw Results table for the inserted answer.
SELECT *
FROM dbo.Results
WHERE SessionID = 1;
GO

-- Example: call the percentage function directly.
SELECT dbo.fn_GetSessionPercent(1, 1) AS ExampleScorePercent;
GO

-- Example: check that the trigger updated the session totals.
SELECT *
FROM QuizSessions
WHERE SessionID = 1;
GO

-- Example: verify a single function call for answer correctness.
SELECT dbo.fn_IsAnswerCorrect(1, 'B') AS IsCorrectAnswer;
GO
DECLARE @SampleSessionID INT;
DECLARE @SampleUserID INT;
DECLARE @SampleQuestionID INT;

SELECT TOP 1 @SampleSessionID = SessionID, @SampleUserID = UserID
FROM QuizSessions;

SELECT TOP 1 @SampleQuestionID = QuestionID
FROM Questions;

IF @SampleSessionID IS NOT NULL AND @SampleUserID IS NOT NULL AND @SampleQuestionID IS NOT NULL
BEGIN
    EXEC dbo.sp_RecordQuizAnswer
        @SessionID = @SampleSessionID,
        @UserID = @SampleUserID,
        @QuestionID = @SampleQuestionID,
        @SelectedAnswer = 'B';
END;
GO