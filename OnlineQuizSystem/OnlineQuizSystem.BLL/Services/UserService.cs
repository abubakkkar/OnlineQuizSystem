using OnlineQuizSystem.DAL.Repositories;
using OnlineQuizSystem.BLL.Models;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class UserService
    {
        UserRepository repo = new UserRepository();

        public DataTable Login(string email, string pass)
        {
            return repo.Login(email, pass);
        }

        public void Register(string n, string e, string p, int? teacherID = null)
        {
            repo.Register(n, e, p, teacherID);
        }

        // Get all teachers enrolled by a user
        public DataTable GetEnrolledTeachers(int userID)
        {
            return repo.GetUserTeachers(userID);
        }

        // Enroll a user in a teacher
        public void EnrollInTeacher(int userID, int teacherID)
        {
            repo.EnrollInTeacher(userID, teacherID);
        }

        // Remove a user from a teacher
        public void RemoveFromTeacher(int userID, int teacherID)
        {
            repo.RemoveFromTeacher(userID, teacherID);
        }

        // Get all available teachers
        public DataTable GetAllTeachers()
        {
            return repo.GetAllTeachers();
        }

        // Check if user is enrolled in a teacher
        public bool IsEnrolledInTeacher(int userID, int teacherID)
        {
            return repo.IsEnrolledInTeacher(userID, teacherID);
        }
    }
}