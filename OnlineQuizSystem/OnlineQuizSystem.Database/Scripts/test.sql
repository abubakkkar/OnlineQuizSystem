--view tables
select * from Teachers;
select * from Users;
select * from Admins;
select * from Questions;
select * from Sections;
select * from UserSections;
select * from QuizSessions;
select * from UserTeachers;
select * from Results;
-- Verification queries: show indexes created on target tables
PRINT 'Indexes on dbo.QuizSessions:';
SELECT i.name, i.is_unique, i.filter_definition
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('dbo.QuizSessions');

PRINT 'Indexes on dbo.Results:';
SELECT i.name, i.is_unique, i.filter_definition
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('dbo.Results');

GO

-- Example: call the stored procedures to verify they return expected results.

EXEC dbo.sp_GetTeacherInfo @TeacherID = 1;
GO
EXEC dbo.sp_GetStudentInfo @UserID = 1;
GO
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

-- Example: score check that the trigger updated the session totals.
SELECT *
FROM QuizSessions
WHERE SessionID = 1;
GO

--Example: call the table valued fucntion to see the question details.
SELECT * FROM dbo.ufn_GetQuestionDetails(1);

--Example: test the trigger that validates correct option by trying to insert an invalid question.
--positive
INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID)
VALUES ('Test valid trigger', 'A1','B1','C1','D1', 'A', 1, 0);
GO
SELECT TOP(1) QuestionID, QuestionText, CorrectOption FROM Questions
WHERE QuestionText = 'Test valid trigger'
ORDER BY QuestionID DESC;
GO
--negavtive
BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID)
    VALUES ('Test invalid trigger', 'A1','B1','C1','D1', 'X', 1, 0);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS ErrorMessage;
END CATCH
GO

SELECT TOP(5) QuestionID, QuestionText, CorrectOption FROM Questions
WHERE QuestionText = 'Test invalid trigger';
GO

-- Run the automated trigger tests (returns results)
EXEC dbo.sp_RunTriggerValidationTests;
GO
