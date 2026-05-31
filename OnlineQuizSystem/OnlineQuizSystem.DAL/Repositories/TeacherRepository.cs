using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class TeacherRepository
    {
        DBHelper db = new DBHelper();

        public DataTable Login(string email, string pass)
        {
            return db.Select($"SELECT * FROM Teachers WHERE Email='{email}' AND Password='{pass}'");
        }

        public void Register(string name, string email, string password)
        {
            db.Execute($"INSERT INTO Teachers (Name, Email, Password, Role) VALUES ('{name}', '{email}', '{password}', 'Teacher')");
        }

        public DataTable GetAllTeachers()
        {
            return db.Select("SELECT TeacherID, Name FROM Teachers");
        }

        public DataTable GetByEmail(string email)
        {
            return db.Select($"SELECT * FROM Teachers WHERE Email='{email}'");
        }

        public DataTable GetTeacherResults(int teacherId)
        {
            return db.Select($@"
                SELECT 
                    s.SessionID, 
                    u.Name as StudentName, 
                    u.Email as StudentEmail, 
                    -- compute total questions dynamically to avoid stale/miscounted values
                    CASE WHEN s.QuizID IS NOT NULL THEN ISNULL((SELECT COUNT(*) FROM QuizQuestions qq WHERE qq.QuizID = s.QuizID), 0)
                         ELSE ISNULL((SELECT COUNT(*) FROM Questions q WHERE (s.TeacherID IS NULL OR q.TeacherID = s.TeacherID) AND (q.SectionID IS NULL OR q.SectionID IN (SELECT us.SectionID FROM UserSections us WHERE us.UserID = s.UserID))), 0)
                    END AS TotalQuestions,
                    s.Score as TotalMarks, 
                    s.StartTime 
                FROM QuizSessions s 
                JOIN Users u ON s.UserID = u.UserID 
                WHERE s.TeacherID = {teacherId} 
                AND s.IsSubmitted = 1
                AND s.SessionID = (
                    SELECT TOP 1 s2.SessionID 
                    FROM QuizSessions s2 
                    WHERE s2.UserID = s.UserID 
                    AND s2.TeacherID = s.TeacherID 
                    AND s2.IsSubmitted = 1 
                    ORDER BY s2.StartTime DESC
                )
                ORDER BY s.StartTime DESC");
        }
    }
}