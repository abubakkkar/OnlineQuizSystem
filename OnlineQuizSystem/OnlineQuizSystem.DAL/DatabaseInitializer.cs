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
                    Name NVARCHAR(100),
                    Email NVARCHAR(100) UNIQUE,
                    Password NVARCHAR(100),
                    Role NVARCHAR(20),
                    TeacherID INT NULL,
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
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID)
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
                    Score INT,
                    StartTime DATETIME,
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
                );";

                new SqlCommand(teachersTable, con).ExecuteNonQuery();
                new SqlCommand(usersTable, con).ExecuteNonQuery();
                new SqlCommand(adminsTable, con).ExecuteNonQuery();
                new SqlCommand(questionsTable, con).ExecuteNonQuery();
                new SqlCommand(sectionsTable, con).ExecuteNonQuery();
                new SqlCommand(userSectionsTable, con).ExecuteNonQuery();
                new SqlCommand(quizSessionsTable, con).ExecuteNonQuery();
                new SqlCommand(userTeachersTable, con).ExecuteNonQuery();
                new SqlCommand(resultsTable, con).ExecuteNonQuery();

                // Insert admin if not exists
                string adminCheck = "IF NOT EXISTS (SELECT * FROM Admins WHERE Email='admin@brainspark.com') INSERT INTO Admins (Name, Email, Password) VALUES ('Admin', 'admin@brainspark.com', 'admin123');";
                new SqlCommand(adminCheck, con).ExecuteNonQuery();
                
                // Add sample teachers
                string teacher1 = "IF NOT EXISTS (SELECT * FROM Teachers WHERE Email='teacher1@school.com') INSERT INTO Teachers (Name, Email, Password) VALUES ('Teacher One', 'teacher1@school.com', 'teacher123');";
                string teacher2 = "IF NOT EXISTS (SELECT * FROM Teachers WHERE Email='teacher2@school.com') INSERT INTO Teachers (Name, Email, Password) VALUES ('Teacher Two', 'teacher2@school.com', 'teacher123');";
                new SqlCommand(teacher1, con).ExecuteNonQuery();
                new SqlCommand(teacher2, con).ExecuteNonQuery();            }
        }
    }
}
