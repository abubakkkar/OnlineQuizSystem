using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class SectionService
    {
        SectionRepository repo = new SectionRepository();

        // Get all sections for a teacher
        public DataTable GetTeacherSections(int teacherID)
        {
            return repo.GetTeacherSections(teacherID);
        }

        // Get all sections
        public DataTable GetAllSections()
        {
            return repo.GetAllSections();
        }

        // Get section details
        public DataTable GetSection(int sectionID)
        {
            return repo.GetSection(sectionID);
        }

        // Create a new section
        public int CreateSection(int teacherID, string sectionName, string description, int maxTimeMinutes = 30)
        {
            return repo.CreateSection(teacherID, sectionName, description, maxTimeMinutes);
        }

        // Update section
        public void UpdateSection(int sectionID, string sectionName, string description, int maxTimeMinutes = 30)
        {
            repo.UpdateSection(sectionID, sectionName, description, maxTimeMinutes);
        }

        // Delete section
        public void DeleteSection(int sectionID)
        {
            repo.DeleteSection(sectionID);
        }

        // Get user's enrolled sections
        public DataTable GetUserSections(int userID)
        {
            return repo.GetUserSections(userID);
        }

        // Enroll user in section
        public void EnrollUserInSection(int userID, int sectionID)
        {
            repo.EnrollUserInSection(userID, sectionID);
        }

        // Remove user from section
        public void RemoveUserFromSection(int userID, int sectionID)
        {
            repo.RemoveUserFromSection(userID, sectionID);
        }

        // Check if user is enrolled in section
        public bool IsUserEnrolledInSection(int userID, int sectionID)
        {
            return repo.IsUserEnrolledInSection(userID, sectionID);
        }

        // Get sections for a specific teacher that user is NOT enrolled in
        public DataTable GetTeacherSectionsNotEnrolled(int userID, int teacherID)
        {
            return repo.GetTeacherSectionsNotEnrolled(userID, teacherID);
        }

        // Get available sections from enrolled teachers
        public DataTable GetAvailableSectionsFromEnrolledTeachers(int userID)
        {
            return repo.GetAvailableSectionsFromEnrolledTeachers(userID);
        }
    }
}
