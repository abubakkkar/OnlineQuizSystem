using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Teacher
{
    public class ProfileModel : PageModel
    {
        private readonly ProfileService _profileService;

        public ProfileModel()
        {
            _profileService = new ProfileService();
        }

        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }
        public string TeacherRole { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TeacherID")) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToPage("/Auth/Login");
            }

            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/Login");
            }

            LoadProfileData(teacherID);
            return Page();
        }

        public IActionResult OnPostUpdateProfile(string Name, string Email)
        {
            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _profileService.UpdateTeacherProfile(teacherID, Name, Email);
                
                // Update session
                HttpContext.Session.SetString("Name", Name);
                HttpContext.Session.SetString("Email", Email);
                
                SuccessMessage = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update profile: " + ex.Message;
            }

            LoadProfileData(teacherID);
            return Page();
        }

        public IActionResult OnPostChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                LoadProfileData(teacherID);
                return Page();
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "New password must be at least 6 characters long.";
                LoadProfileData(teacherID);
                return Page();
            }

            try
            {
                bool changed = _profileService.ChangeTeacherPassword(teacherID, CurrentPassword, NewPassword);
                if (changed)
                {
                    SuccessMessage = "Password changed successfully!";
                }
                else
                {
                    ErrorMessage = "Current password is incorrect.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to change password: " + ex.Message;
            }

            LoadProfileData(teacherID);
            return Page();
        }

        private void LoadProfileData(int teacherID)
        {
            var profileData = _profileService.GetTeacherProfile(teacherID);
            if (profileData.Rows.Count > 0)
            {
                TeacherName = profileData.Rows[0]["Name"].ToString();
                TeacherEmail = profileData.Rows[0]["Email"].ToString();
                TeacherRole = profileData.Rows[0]["Role"].ToString();
            }
        }
    }
}
