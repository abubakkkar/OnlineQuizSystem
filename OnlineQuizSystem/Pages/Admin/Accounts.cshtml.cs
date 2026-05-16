using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.Pages.Admin
{
    public class AccountsModel : PageModel
    {
        private readonly AdminRepository _adminRepo;

        public AccountsModel()
        {
            _adminRepo = new AdminRepository();
        }

        public DataTable Teachers { get; set; }
        public DataTable Users { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
            {
                return RedirectToPage("/Auth/AdminLogin");
            }

            Teachers = _adminRepo.GetAllTeachers();
            Users = _adminRepo.GetAllUsers();

            return Page();
        }

        public IActionResult OnPostToggleTeacherStatus(int teacherId, bool isActive)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
            {
                return RedirectToPage("/Auth/AdminLogin");
            }

            _adminRepo.ToggleTeacherStatus(teacherId, isActive);
            SuccessMessage = $"Teacher account {(isActive ? "enabled" : "disabled")} successfully.";
            
            return RedirectToPage();
        }

        public IActionResult OnPostToggleUserStatus(int userId, bool isActive)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
            {
                return RedirectToPage("/Auth/AdminLogin");
            }

            _adminRepo.ToggleUserStatus(userId, isActive);
            SuccessMessage = $"User account {(isActive ? "enabled" : "disabled")} successfully.";
            
            return RedirectToPage();
        }
    }
}
