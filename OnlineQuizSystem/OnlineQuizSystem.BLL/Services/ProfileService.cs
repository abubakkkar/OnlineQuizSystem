using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class ProfileService
    {
        ProfileRepository repo = new ProfileRepository();

        // User Profile Methods
        public DataTable GetUserProfile(int userID)
        {
            return repo.GetUserProfile(userID);
        }

        public void UpdateProfile(int userID, string name, string email)
        {
            repo.UpdateProfile(userID, name, email);
        }

        public bool ChangePassword(int userID, string currentPassword, string newPassword)
        {
            return repo.ChangePassword(userID, currentPassword, newPassword);
        }

        public bool VerifyPassword(int userID, string password)
        {
            return repo.VerifyPassword(userID, password);
        }

        // Teacher Profile Methods
        public DataTable GetTeacherProfile(int teacherID)
        {
            return repo.GetTeacherProfile(teacherID);
        }

        public void UpdateTeacherProfile(int teacherID, string name, string email)
        {
            repo.UpdateTeacherProfile(teacherID, name, email);
        }

        public bool ChangeTeacherPassword(int teacherID, string currentPassword, string newPassword)
        {
            return repo.ChangeTeacherPassword(teacherID, currentPassword, newPassword);
        }

        public bool VerifyTeacherPassword(int teacherID, string password)
        {
            return repo.VerifyTeacherPassword(teacherID, password);
        }
    }
}
