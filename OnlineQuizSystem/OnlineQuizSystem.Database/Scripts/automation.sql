
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

-- Procedure: run simple trigger validation tests for dbo.trg_ValidateCorrectOption
CREATE PROCEDURE dbo.sp_RunTriggerValidationTests
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Results TABLE (
        TestName NVARCHAR(100),
        Success BIT,
        Message NVARCHAR(4000),
        CreatedQuestionID INT
    );

    -- Test 1: invalid insert (CorrectOption not in A-D) -> should error
    BEGIN TRY
        INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID, SectionID)
        VALUES ('_TRIGGER_TEST_INVALID', 'A','B','C','D', 'X', 1, (SELECT TOP 1 TeacherID FROM Teachers), (SELECT TOP 1 SectionID FROM Sections));

        -- If insert succeeded, record failure and remove row
        INSERT INTO @Results VALUES ('Invalid Insert', 0, 'Expected failure but insert succeeded', NULL);
        DELETE FROM Questions WHERE QuestionText = '_TRIGGER_TEST_INVALID';
    END TRY
    BEGIN CATCH
        INSERT INTO @Results (TestName, Success, Message, CreatedQuestionID)
        VALUES ('Invalid Insert', 1, ERROR_MESSAGE(), NULL);
    END CATCH;

    -- Test 2: valid insert (should succeed)
    DECLARE @NewID INT = NULL;
    BEGIN TRY
        INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID, SectionID)
        VALUES ('_TRIGGER_TEST_VALID', 'A','B','C','D', 'A', 1, (SELECT TOP 1 TeacherID FROM Teachers), (SELECT TOP 1 SectionID FROM Sections));
        SET @NewID = CAST(SCOPE_IDENTITY() AS INT);
        INSERT INTO @Results VALUES ('Valid Insert', 1, 'Inserted successfully', @NewID);
    END TRY
    BEGIN CATCH
        INSERT INTO @Results VALUES ('Valid Insert', 0, ERROR_MESSAGE(), NULL);
    END CATCH;

    -- Test 3: update to invalid value (should error)
    BEGIN TRY
        IF @NewID IS NOT NULL
        BEGIN
            UPDATE Questions SET CorrectOption = 'Z' WHERE QuestionID = @NewID;
            INSERT INTO @Results VALUES ('Invalid Update', 0, 'Expected failure but update succeeded', @NewID);
            -- attempt to restore
            UPDATE Questions SET CorrectOption = 'A' WHERE QuestionID = @NewID;
        END
        ELSE
        BEGIN
            INSERT INTO @Results VALUES ('Invalid Update', 0, 'Skipped because valid insert failed', NULL);
        END
    END TRY
    BEGIN CATCH
        INSERT INTO @Results VALUES ('Invalid Update', 1, ERROR_MESSAGE(), @NewID);
    END CATCH;

    -- Cleanup created test row
    IF @NewID IS NOT NULL
        DELETE FROM Questions WHERE QuestionID = @NewID;

    -- Return results
    SELECT TestName, Success, Message, CreatedQuestionID FROM @Results;
END;
GO

