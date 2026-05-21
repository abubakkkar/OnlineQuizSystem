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
                    s.TotalQuestions, 
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