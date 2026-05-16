using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly ProfileService _profileService;
        private readonly UserService _userService;

        public IndexModel()
        {
            _profileService = new ProfileService();
            _userService = new UserService();
        }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public DataTable EnrolledTeachers { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                return RedirectToPage("/Auth/Login");
            }

            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            LoadProfileData(userID);
            return Page();
        }

        public IActionResult OnPostUpdateProfile(string Name, string Email)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _profileService.UpdateProfile(userID, Name, Email);
                
                // Update session
                HttpContext.Session.SetString("Name", Name);
                HttpContext.Session.SetString("Email", Email);
                
                SuccessMessage = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update profile: " + ex.Message;
            }

            LoadProfileData(userID);
            return Page();
        }

        public IActionResult OnPostChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                LoadProfileData(userID);
                return Page();
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "New password must be at least 6 characters long.";
                LoadProfileData(userID);
                return Page();
            }

            try
            {
                bool changed = _profileService.ChangePassword(userID, CurrentPassword, NewPassword);
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

            LoadProfileData(userID);
            return Page();
        }

        public IActionResult OnPostRemoveTeacher(int teacherId)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _userService.RemoveFromTeacher(userID, teacherId);
                SuccessMessage = "Successfully removed from teacher.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to remove from teacher: " + ex.Message;
            }

            LoadProfileData(userID);
            return Page();
        }

        private void LoadProfileData(int userID)
        {
            var profileData = _profileService.GetUserProfile(userID);
            if (profileData.Rows.Count > 0)
            {
                UserName = profileData.Rows[0]["Name"].ToString();
                UserEmail = profileData.Rows[0]["Email"].ToString();
                UserRole = profileData.Rows[0]["Role"].ToString();
            }

            EnrolledTeachers = _userService.GetEnrolledTeachers(userID);
        }
    }
}
