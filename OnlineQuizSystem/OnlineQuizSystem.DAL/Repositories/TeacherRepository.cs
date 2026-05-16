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
                    COUNT(r.ResultID) as TotalQuestions, 
                    ISNULL(SUM(CAST(r.IsCorrect as INT)), 0) as TotalMarks, 
                    s.StartTime 
                FROM QuizSessions s 
                JOIN Users u ON s.UserID = u.UserID 
                LEFT JOIN Results r ON s.SessionID = r.SessionID 
                WHERE s.TeacherID = {teacherId} 
                GROUP BY s.SessionID, u.Name, u.Email, s.StartTime
                ORDER BY s.StartTime DESC");
        }
    }
}