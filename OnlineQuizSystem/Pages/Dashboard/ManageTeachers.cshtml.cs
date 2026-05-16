using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Dashboard
{
    public class ManageTeachersModel : PageModel
    {
        private readonly UserService _userService;
        private readonly TeacherService _teacherService;

        public ManageTeachersModel()
        {
            _userService = new UserService();
            _teacherService = new TeacherService();
        }

        public DataTable EnrolledTeachers { get; set; }
        public DataTable AvailableTeachers { get; set; }

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

            EnrolledTeachers = _userService.GetEnrolledTeachers(userID);
            AvailableTeachers = _userService.GetAllTeachers();

            return Page();
        }

        public IActionResult OnPostEnroll(int teacherId)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIDStr, out int userID))
            {
                _userService.EnrollInTeacher(userID, teacherId);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int teacherId)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIDStr, out int userID))
            {
                _userService.RemoveFromTeacher(userID, teacherId);
            }

            return RedirectToPage();
        }
    }
}
