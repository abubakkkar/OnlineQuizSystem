using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class UserRepository
    {
        DBHelper db = new DBHelper();

        public DataTable Login(string email, string pass)
        {
            return db.Select($"SELECT * FROM Users WHERE Email='{email}' AND Password='{pass}'");
        }

        public void Register(string name, string email, string password, string rollNo, int? teacherID = null)
        {
            string teacherIDValue = teacherID.HasValue ? teacherID.Value.ToString() : "NULL";
            db.Execute($"INSERT INTO Users (RollNo, Name, Email, Password, Role, TeacherID) VALUES ('{rollNo}', '{name}', '{email}', '{password}', 'Student', {teacherIDValue})");
        }

        // Get all teachers enrolled by a user
        public DataTable GetUserTeachers(int userID)
        {
            return db.Select($"SELECT t.* FROM Teachers t INNER JOIN UserTeachers ut ON t.TeacherID = ut.TeacherID WHERE ut.UserID = {userID}");
        }

        // Enroll a user in a teacher
        public void EnrollInTeacher(int userID, int teacherID)
        {
            // Check if already enrolled
            DataTable check = db.Select($"SELECT * FROM UserTeachers WHERE UserID = {userID} AND TeacherID = {teacherID}");
            if (check.Rows.Count == 0)
            {
                db.Execute($"INSERT INTO UserTeachers (UserID, TeacherID) VALUES ({userID}, {teacherID})");
            }
        }

        // Remove a user from a teacher
        public void RemoveFromTeacher(int userID, int teacherID)
        {
            db.Execute($"DELETE FROM UserTeachers WHERE UserID = {userID} AND TeacherID = {teacherID}");
        }

        // Get all available teachers
        public DataTable GetAllTeachers()
        {
            return db.Select("SELECT * FROM Teachers");
        }

        // Check if user is enrolled in a teacher
        public bool IsEnrolledInTeacher(int userID, int teacherID)
        {
            DataTable dt = db.Select($"SELECT * FROM UserTeachers WHERE UserID = {userID} AND TeacherID = {teacherID}");
            return dt.Rows.Count > 0;
        }
    }
}