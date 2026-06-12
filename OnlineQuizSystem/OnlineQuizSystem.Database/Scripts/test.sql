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
--index verification query: list all indexes on user tables with details about key and included columns
SELECT
  s.name AS SchemaName,
  t.name AS TableName,
  i.name AS IndexName,
  i.type_desc,
  i.is_unique,
  i.is_primary_key,
  STUFF((
    SELECT ', ' + c.name
    FROM sys.index_columns ic
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0
    ORDER BY ic.key_ordinal
    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'),1,2,'') AS KeyColumns,
  STUFF((
    SELECT ', ' + c.name
    FROM sys.index_columns ic
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 1
    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'),1,2,'') AS IncludedColumns,
  i.fill_factor
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name, i.name;
 --trigger 
  insert into Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID)
 values ('Sample Question', '', '', '', '','',1,1);