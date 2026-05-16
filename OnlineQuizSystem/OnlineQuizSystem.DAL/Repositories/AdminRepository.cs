using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class AdminRepository
    {
        DBHelper db = new DBHelper();

        public DataTable Login(string email, string pass)
        {
            return db.Select($"SELECT * FROM Admins WHERE Email='{email}' AND Password='{pass}'");
        }

        public DataTable GetByEmail(string email)
        {
            return db.Select($"SELECT * FROM Admins WHERE Email='{email}'");
        }

        public void Register(string name, string email, string pass)
        {
            db.Execute($"INSERT INTO Admins (Name, Email, Password, Role) VALUES('{name}','{email}','{pass}','Admin')");
        }

        public DataTable GetAllTeachers()
        {
            return db.Select("SELECT TeacherID, Name, Email, IsActive FROM Teachers");
        }

        public DataTable GetAllUsers()
        {
            return db.Select("SELECT UserID, Name, Email, Role, IsActive FROM Users");
        }

        public void ToggleTeacherStatus(int teacherId, bool isActive)
        {
            int bitValue = isActive ? 1 : 0;
            db.Execute($"UPDATE Teachers SET IsActive = {bitValue} WHERE TeacherID = {teacherId}");
        }

        public void ToggleUserStatus(int userId, bool isActive)
        {
            int bitValue = isActive ? 1 : 0;
            db.Execute($"UPDATE Users SET IsActive = {bitValue} WHERE UserID = {userId}");
        }
        
        public DataTable GetAllResults()
        {
            return db.Select(@"
                SELECT r.ResultID, u.Name as StudentName, q.QuestionText, r.SelectedAnswer, r.IsCorrect, r.AnsweredAt
                FROM Results r
                JOIN Users u ON r.UserID = u.UserID
                JOIN Questions q ON r.QuestionID = q.QuestionID
                ORDER BY r.AnsweredAt DESC");
        }
    }
}
