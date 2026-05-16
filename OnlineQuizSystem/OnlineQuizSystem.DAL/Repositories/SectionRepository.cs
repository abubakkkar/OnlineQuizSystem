using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class SectionRepository
    {
        DBHelper db = new DBHelper();

        // Get all sections for a teacher
        public DataTable GetTeacherSections(int teacherID)
        {
            return db.Select($"SELECT * FROM Sections WHERE TeacherID = {teacherID} ORDER BY SectionName");
        }

        // Get all sections
        public DataTable GetAllSections()
        {
            return db.Select("SELECT s.*, t.Name as TeacherName FROM Sections s INNER JOIN Teachers t ON s.TeacherID = t.TeacherID ORDER BY t.Name, s.SectionName");
        }

        // Get section details
        public DataTable GetSection(int sectionID)
        {
            return db.Select($"SELECT * FROM Sections WHERE SectionID = {sectionID}");
        }

        // Create a new section
        public int CreateSection(int teacherID, string sectionName, string description, int maxTimeMinutes = 30)
        {
            DataTable dt = db.Select($"INSERT INTO Sections (SectionName, TeacherID, Description, MaxTimeMinutes) VALUES ('{sectionName}', {teacherID}, '{description}', {maxTimeMinutes}); SELECT SCOPE_IDENTITY() as SectionID");
            if (dt.Rows.Count > 0 && dt.Rows[0]["SectionID"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["SectionID"]);
            }
            return 0;
        }

        // Update section
        public void UpdateSection(int sectionID, string sectionName, string description, int maxTimeMinutes = 30)
        {
            db.Execute($"UPDATE Sections SET SectionName = '{sectionName}', Description = '{description}', MaxTimeMinutes = {maxTimeMinutes} WHERE SectionID = {sectionID}");
        }

        // Delete section
        public void DeleteSection(int sectionID)
        {
            // Remove all user enrollments first
            db.Execute($"DELETE FROM UserSections WHERE SectionID = {sectionID}");
            // Delete the section
            db.Execute($"DELETE FROM Sections WHERE SectionID = {sectionID}");
        }

        // Get user's enrolled sections
        public DataTable GetUserSections(int userID)
        {
            return db.Select($"SELECT s.*, t.Name as TeacherName FROM Sections s INNER JOIN UserSections us ON s.SectionID = us.SectionID INNER JOIN Teachers t ON s.TeacherID = t.TeacherID WHERE us.UserID = {userID} ORDER BY t.Name, s.SectionName");
        }

        // Enroll user in section
        public void EnrollUserInSection(int userID, int sectionID)
        {
            // Check if already enrolled
            DataTable check = db.Select($"SELECT * FROM UserSections WHERE UserID = {userID} AND SectionID = {sectionID}");
            if (check.Rows.Count == 0)
            {
                db.Execute($"INSERT INTO UserSections (UserID, SectionID) VALUES ({userID}, {sectionID})");
            }
        }

        // Remove user from section
        public void RemoveUserFromSection(int userID, int sectionID)
        {
            db.Execute($"DELETE FROM UserSections WHERE UserID = {userID} AND SectionID = {sectionID}");
        }

        // Check if user is enrolled in section
        public bool IsUserEnrolledInSection(int userID, int sectionID)
        {
            DataTable dt = db.Select($"SELECT * FROM UserSections WHERE UserID = {userID} AND SectionID = {sectionID}");
            return dt.Rows.Count > 0;
        }

        // Get sections for a specific teacher that user is NOT enrolled in
        public DataTable GetTeacherSectionsNotEnrolled(int userID, int teacherID)
        {
            return db.Select($"SELECT * FROM Sections WHERE TeacherID = {teacherID} AND SectionID NOT IN (SELECT SectionID FROM UserSections WHERE UserID = {userID})");
        }

        // Get available sections from enrolled teachers
        public DataTable GetAvailableSectionsFromEnrolledTeachers(int userID)
        {
            return db.Select($@"
                SELECT s.*, t.Name as TeacherName 
                FROM Sections s 
                INNER JOIN Teachers t ON s.TeacherID = t.TeacherID
                INNER JOIN UserTeachers ut ON s.TeacherID = ut.TeacherID
                WHERE ut.UserID = {userID}
                AND s.SectionID NOT IN (SELECT SectionID FROM UserSections WHERE UserID = {userID})
                ORDER BY t.Name, s.SectionName
            ");
        }
    }
}
