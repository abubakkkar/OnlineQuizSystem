using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class ProfileRepository
    {
        DBHelper db = new DBHelper();

        // Get user profile by ID
        public DataTable GetUserProfile(int userID)
        {
            return db.Select($"SELECT UserID, Name, Email, Role FROM Users WHERE UserID = {userID}");
        }

        // Update user profile (name)
        public void UpdateProfile(int userID, string name, string email)
        {
            db.Execute($"UPDATE Users SET Name = '{name}', Email = '{email}' WHERE UserID = {userID}");
        }

        // Change password
        public bool ChangePassword(int userID, string currentPassword, string newPassword)
        {
            // First verify the current password
            DataTable dt = db.Select($"SELECT * FROM Users WHERE UserID = {userID} AND Password = '{currentPassword}'");
            if (dt.Rows.Count > 0)
            {
                // Update to new password
                db.Execute($"UPDATE Users SET Password = '{newPassword}' WHERE UserID = {userID}");
                return true;
            }
            return false;
        }

        // Verify password
        public bool VerifyPassword(int userID, string password)
        {
            DataTable dt = db.Select($"SELECT * FROM Users WHERE UserID = {userID} AND Password = '{password}'");
            return dt.Rows.Count > 0;
        }

        // Get teacher profile by ID
        public DataTable GetTeacherProfile(int teacherID)
        {
            return db.Select($"SELECT TeacherID, Name, Email, Role FROM Teachers WHERE TeacherID = {teacherID}");
        }

        // Update teacher profile
        public void UpdateTeacherProfile(int teacherID, string name, string email)
        {
            db.Execute($"UPDATE Teachers SET Name = '{name}', Email = '{email}' WHERE TeacherID = {teacherID}");
        }

        // Change teacher password
        public bool ChangeTeacherPassword(int teacherID, string currentPassword, string newPassword)
        {
            DataTable dt = db.Select($"SELECT * FROM Teachers WHERE TeacherID = {teacherID} AND Password = '{currentPassword}'");
            if (dt.Rows.Count > 0)
            {
                db.Execute($"UPDATE Teachers SET Password = '{newPassword}' WHERE TeacherID = {teacherID}");
                return true;
            }
            return false;
        }

        // Verify teacher password
        public bool VerifyTeacherPassword(int teacherID, string password)
        {
            DataTable dt = db.Select($"SELECT * FROM Teachers WHERE TeacherID = {teacherID} AND Password = '{password}'");
            return dt.Rows.Count > 0;
        }
    }
}
