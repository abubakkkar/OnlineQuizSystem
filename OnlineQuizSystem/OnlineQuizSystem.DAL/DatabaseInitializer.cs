using System.Data.SqlClient;

namespace OnlineQuizSystem.DAL
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            string masterCon = "Server=.;Database=master;Trusted_Connection=True;";
            
            using (var con = new SqlConnection(masterCon))
            {
                con.Open();
                
                var cmdDb = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OnlineQuizDB') CREATE DATABASE OnlineQuizDB;", con);
                cmdDb.ExecuteNonQuery();
            }

            string quizCon = "Server=.;Database=OnlineQuizDB;Trusted_Connection=True;";
            using (var con = new SqlConnection(quizCon))
            {
                con.Open();

                string teachersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Teachers' and xtype='U')
                CREATE TABLE Teachers (
                    TeacherID INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100),
                    Email NVARCHAR(100) UNIQUE,
                    Password NVARCHAR(100),
                    Role NVARCHAR(20) DEFAULT 'Teacher'
                );";

                string usersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
                CREATE TABLE Users (
                    UserID INT PRIMARY KEY IDENTITY(1,1),
                    RollNo NVARCHAR(50) NOT NULL UNIQUE,
                    Name NVARCHAR(100),
                    Email NVARCHAR(100) UNIQUE,
                    Password NVARCHAR(100),
                    Role NVARCHAR(20),
                    TeacherID INT NULL,
                    IsActive BIT DEFAULT 1,
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID)
                );";

                string adminsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Admins' and xtype='U')
                CREATE TABLE Admins (
                    AdminID INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100),
                    Email NVARCHAR(100) UNIQUE,
                    Password NVARCHAR(100),
                    Role NVARCHAR(20) DEFAULT 'Admin'
                );";

                string questionsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Questions' and xtype='U')
                CREATE TABLE Questions (
                    QuestionID INT PRIMARY KEY IDENTITY(1,1),
                    QuestionText NVARCHAR(MAX),
                    OptionA NVARCHAR(200),
                    OptionB NVARCHAR(200),
                    OptionC NVARCHAR(200),
                    OptionD NVARCHAR(200),
                    CorrectOption CHAR(1),
                    DifficultyLevel INT,
                    TeacherID INT,
                    SectionID INT NULL,
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID),
                    FOREIGN KEY(SectionID) REFERENCES Sections(SectionID)
                );";

                string sectionsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sections' and xtype='U')
                CREATE TABLE Sections (
                    SectionID INT PRIMARY KEY IDENTITY(1,1),
                    SectionName NVARCHAR(100),
                    TeacherID INT NOT NULL,
                    Description NVARCHAR(MAX),
                    CreatedDate DATETIME DEFAULT GETDATE(),
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID)
                );";

                string quizzesTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Quizzes' and xtype='U')
                CREATE TABLE Quizzes (
                    QuizID INT PRIMARY KEY IDENTITY(1,1),
                    SectionID INT NULL,
                    TeacherID INT NULL,
                    Title NVARCHAR(200),
                    MaxTimeMinutes INT DEFAULT 30,
                    CreatedDate DATETIME DEFAULT GETDATE(),
                    FOREIGN KEY(SectionID) REFERENCES Sections(SectionID),
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID)
                );";

                string quizQuestionsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='QuizQuestions' and xtype='U')
                CREATE TABLE QuizQuestions (
                    QuizQuestionID INT PRIMARY KEY IDENTITY(1,1),
                    QuizID INT NOT NULL,
                    QuestionID INT NOT NULL,
                    FOREIGN KEY(QuizID) REFERENCES Quizzes(QuizID),
                    FOREIGN KEY(QuestionID) REFERENCES Questions(QuestionID)
                );";

                string userSectionsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserSections' and xtype='U')
                CREATE TABLE UserSections (
                    UserSectionID INT PRIMARY KEY IDENTITY(1,1),
                    UserID INT NOT NULL,
                    SectionID INT NOT NULL,
                    EnrolledDate DATETIME DEFAULT GETDATE(),
                    FOREIGN KEY(UserID) REFERENCES Users(UserID),
                    FOREIGN KEY(SectionID) REFERENCES Sections(SectionID),
                    UNIQUE(UserID, SectionID)
                );";

                string quizSessionsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='QuizSessions' and xtype='U')
                CREATE TABLE QuizSessions (
                    SessionID INT PRIMARY KEY IDENTITY(1,1),
                    UserID INT,
                    TeacherID INT,
                    QuizID INT NULL,
                    Score INT,
                    StartTime DATETIME,
                    EndTime DATETIME,
                    MaxTimeMinutes INT DEFAULT 30,
                    TotalQuestions INT DEFAULT 0,
                    IsSubmitted BIT DEFAULT 0,
                    FOREIGN KEY(UserID) REFERENCES Users(UserID),
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID)
                );";

                string userTeachersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserTeachers' and xtype='U')
                CREATE TABLE UserTeachers (
                    UserTeacherID INT PRIMARY KEY IDENTITY(1,1),
                    UserID INT NOT NULL,
                    TeacherID INT NOT NULL,
                    EnrolledDate DATETIME DEFAULT GETDATE(),
                    FOREIGN KEY(UserID) REFERENCES Users(UserID),
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID),
                    UNIQUE(UserID, TeacherID)
                );";

                string resultsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Results' and xtype='U')
                CREATE TABLE Results (
                    ResultID INT PRIMARY KEY IDENTITY(1,1),
                    UserID INT,
                    QuestionID INT,
                    SelectedAnswer CHAR(1),
                    IsCorrect BIT,
                    AnsweredAt DATETIME DEFAULT GETDATE(),
                    SessionID INT,
                    FOREIGN KEY(UserID) REFERENCES Users(UserID),
                    FOREIGN KEY(QuestionID) REFERENCES Questions(QuestionID),
                    FOREIGN KEY(SessionID) REFERENCES QuizSessions(SessionID)
                );
                -- Ensure unique answers per session/question (prevent duplicates at DB level)
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Results_Session_Question' AND object_id = OBJECT_ID('Results'))
                    CREATE UNIQUE INDEX UX_Results_Session_Question ON Results(SessionID, QuestionID);";

                new SqlCommand(teachersTable, con).ExecuteNonQuery();
                new SqlCommand(usersTable, con).ExecuteNonQuery();
                new SqlCommand(adminsTable, con).ExecuteNonQuery();
                new SqlCommand(sectionsTable, con).ExecuteNonQuery();
                new SqlCommand(questionsTable, con).ExecuteNonQuery();
                new SqlCommand(userSectionsTable, con).ExecuteNonQuery();
                new SqlCommand(quizSessionsTable, con).ExecuteNonQuery();
                new SqlCommand(userTeachersTable, con).ExecuteNonQuery();
                new SqlCommand(resultsTable, con).ExecuteNonQuery();

                string addRollNoColumn = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Users' AND COLUMN_NAME='RollNo')
                BEGIN
                    ALTER TABLE Users ADD RollNo NVARCHAR(50) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_RollNo' AND object_id = OBJECT_ID('Users'))
                        CREATE UNIQUE INDEX IX_Users_RollNo ON Users(RollNo);
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_RollNo' AND object_id = OBJECT_ID('Users'))
                        CREATE UNIQUE INDEX IX_Users_RollNo ON Users(RollNo);
                END";

                new SqlCommand(addRollNoColumn, con).ExecuteNonQuery();

                // Add missing columns to existing tables (migration)
                string addQuizSessionsColumns = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='QuizSessions' AND COLUMN_NAME='EndTime')
                    ALTER TABLE QuizSessions ADD EndTime DATETIME;
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='QuizSessions' AND COLUMN_NAME='MaxTimeMinutes')
                    ALTER TABLE QuizSessions ADD MaxTimeMinutes INT DEFAULT 30;
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='QuizSessions' AND COLUMN_NAME='QuizID')
                    ALTER TABLE QuizSessions ADD QuizID INT NULL;
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='QuizSessions' AND COLUMN_NAME='TotalQuestions')
                    ALTER TABLE QuizSessions ADD TotalQuestions INT DEFAULT 0;
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='QuizSessions' AND COLUMN_NAME='IsSubmitted')
                    ALTER TABLE QuizSessions ADD IsSubmitted BIT DEFAULT 0;";                
                string addQuestionsColumn = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Questions' AND COLUMN_NAME='SectionID')
                    ALTER TABLE Questions ADD SectionID INT NULL;";

                string addSectionsColumn = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Sections' AND COLUMN_NAME='MaxTimeMinutes')
                    ALTER TABLE Sections ADD MaxTimeMinutes INT DEFAULT 30;";

                string addQuizzesAndMappings = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Quizzes')
                BEGIN
                    CREATE TABLE Quizzes (QuizID INT PRIMARY KEY IDENTITY(1,1), SectionID INT NULL, TeacherID INT NULL, Title NVARCHAR(200), MaxTimeMinutes INT DEFAULT 30, CreatedDate DATETIME DEFAULT GETDATE());
                END
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='QuizQuestions')
                BEGIN
                    CREATE TABLE QuizQuestions (QuizQuestionID INT PRIMARY KEY IDENTITY(1,1), QuizID INT NOT NULL, QuestionID INT NOT NULL);
                END";

                new SqlCommand(addQuizSessionsColumns, con).ExecuteNonQuery();
                new SqlCommand(addQuestionsColumn, con).ExecuteNonQuery();
                new SqlCommand(addSectionsColumn, con).ExecuteNonQuery();
                new SqlCommand(addQuizzesAndMappings, con).ExecuteNonQuery();

                string advancedDbObjects = @"
                EXEC('CREATE OR ALTER FUNCTION dbo.ufn_GetSessionScore(@SessionID INT) RETURNS INT AS BEGIN RETURN ISNULL((SELECT COUNT(*) FROM Results r WHERE r.SessionID = @SessionID AND r.IsCorrect = 1), 0); END');
                EXEC('CREATE OR ALTER FUNCTION dbo.ufn_GetSessionPercentage(@SessionID INT) RETURNS DECIMAL(5,2) AS BEGIN DECLARE @score INT = dbo.ufn_GetSessionScore(@SessionID); DECLARE @total INT = ISNULL((SELECT TotalQuestions FROM QuizSessions WHERE SessionID = @SessionID), 0); RETURN CASE WHEN @total = 0 THEN 0 ELSE CAST(@score * 100.0 / @total AS DECIMAL(5,2)) END; END');
                EXEC('CREATE OR ALTER PROCEDURE dbo.sp_StartQuizSession @UserID INT, @TeacherID INT = NULL, @QuizID INT = NULL, @MaxTimeMinutes INT = 30 AS BEGIN SET NOCOUNT ON; DECLARE @ExistingSessionID INT = NULL; IF @TeacherID IS NOT NULL BEGIN SELECT TOP 1 @ExistingSessionID = SessionID FROM QuizSessions WHERE UserID = @UserID AND TeacherID = @TeacherID AND IsSubmitted = 0 ORDER BY StartTime DESC; END IF @ExistingSessionID IS NOT NULL BEGIN SELECT @ExistingSessionID AS SessionID; RETURN; END DECLARE @TotalQuestions INT = 0; DECLARE @UseMinutes INT = @MaxTimeMinutes; IF @QuizID IS NOT NULL BEGIN SELECT @UseMinutes = ISNULL(MaxTimeMinutes, @MaxTimeMinutes) FROM Quizzes WHERE QuizID = @QuizID; SELECT @TotalQuestions = (SELECT COUNT(*) FROM QuizQuestions qq WHERE qq.QuizID = @QuizID); IF @TotalQuestions = 0 BEGIN DECLARE @sec INT = (SELECT SectionID FROM Quizzes WHERE QuizID = @QuizID); SELECT @TotalQuestions = ISNULL((SELECT COUNT(*) FROM Questions q WHERE q.SectionID = @sec), 0); END END ELSE BEGIN SELECT @TotalQuestions = (SELECT COUNT(*) FROM Questions q WHERE (@TeacherID IS NULL OR q.TeacherID = @TeacherID) AND (@TeacherID IS NULL OR q.SectionID IS NULL OR q.SectionID IN (SELECT us.SectionID FROM UserSections us WHERE us.USERID = @UserID))); END INSERT INTO QuizSessions (UserID, TeacherID, QuizID, Score, StartTime, EndTime, MaxTimeMinutes, TotalQuestions, IsSubmitted) VALUES (@UserID, @TeacherID, @QuizID, 0, GETDATE(), DATEADD(MINUTE, @UseMinutes, GETDATE()), @UseMinutes, @TotalQuestions, 0); SELECT SCOPE_IDENTITY() AS SessionID; END');
                EXEC('CREATE OR ALTER PROCEDURE dbo.sp_SaveQuizResult @UserID INT, @QuestionID INT, @SelectedAnswer CHAR(1), @SessionID INT AS BEGIN SET NOCOUNT ON; IF EXISTS (SELECT 1 FROM Results WHERE SessionID = @SessionID AND QuestionID = @QuestionID) RETURN; DECLARE @CorrectOption CHAR(1) = (SELECT CorrectOption FROM Questions WHERE QuestionID = @QuestionID); DECLARE @IsCorrect BIT = CASE WHEN @CorrectOption = @SelectedAnswer THEN 1 ELSE 0 END; INSERT INTO Results (UserID, QuestionID, SelectedAnswer, IsCorrect, SessionID) VALUES (@UserID, @QuestionID, @SelectedAnswer, @IsCorrect, @SessionID); UPDATE QuizSessions SET Score = dbo.ufn_GetSessionScore(@SessionID) WHERE SessionID = @SessionID; END');
                EXEC('CREATE OR ALTER PROCEDURE dbo.sp_SubmitQuiz @SessionID INT AS BEGIN SET NOCOUNT ON; UPDATE QuizSessions SET Score = dbo.ufn_GetSessionScore(@SessionID), IsSubmitted = 1, EndTime = CASE WHEN EndTime < GETDATE() THEN EndTime ELSE GETDATE() END WHERE SessionID = @SessionID; END');
                EXEC('CREATE OR ALTER PROCEDURE dbo.sp_AutoSubmitExpiredSessions AS BEGIN SET NOCOUNT ON; UPDATE qs SET Score = dbo.ufn_GetSessionScore(qs.SessionID), IsSubmitted = 1 FROM QuizSessions qs WHERE qs.IsSubmitted = 0 AND qs.EndTime <= GETDATE(); END');
";
                // Also add unique index on Results(SessionID, QuestionID) if not already present
                string addResultsUniqueIndex = @"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Results_Session_Question' AND object_id = OBJECT_ID('Results'))
                    CREATE UNIQUE INDEX UX_Results_Session_Question ON Results(SessionID, QuestionID);";
                new SqlCommand(addResultsUniqueIndex, con).ExecuteNonQuery();
                new SqlCommand(advancedDbObjects, con).ExecuteNonQuery();

                // Insert admin if not exists
                string adminCheck = "IF NOT EXISTS (SELECT * FROM Admins WHERE Email='admin@brainspark.com') INSERT INTO Admins (Name, Email, Password) VALUES ('Admin', 'admin@brainspark.com', 'admin123');";
                new SqlCommand(adminCheck, con).ExecuteNonQuery();

                // Enforce one session per student per teacher:
                // Remove duplicate unsubmitted sessions (keep the oldest one per user+teacher),
                // then add a unique filtered index to prevent future duplicates.
                string enforceOneSession = @"
                -- Delete extra unsubmitted sessions, keeping only the earliest one per (UserID, TeacherID)
                WITH RankedSessions AS (
                    SELECT SessionID,
                           ROW_NUMBER() OVER (PARTITION BY UserID, TeacherID ORDER BY StartTime ASC) AS rn
                    FROM QuizSessions
                    WHERE IsSubmitted = 0 AND TeacherID IS NOT NULL
                )
                DELETE FROM QuizSessions WHERE SessionID IN (
                    SELECT SessionID FROM RankedSessions WHERE rn > 1
                );

                -- Add unique filtered index if it doesn't already exist
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'UX_QuizSessions_UserID_TeacherID_Active'
                      AND object_id = OBJECT_ID('QuizSessions')
                )
                BEGIN
                    CREATE UNIQUE INDEX UX_QuizSessions_UserID_TeacherID_Active
                    ON QuizSessions (UserID, TeacherID)
                    WHERE IsSubmitted = 0 AND TeacherID IS NOT NULL;
                END";
                new SqlCommand(enforceOneSession, con).ExecuteNonQuery();

                // Add sample teachers
            }
        }
    }
}
