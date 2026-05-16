using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly UserService _userService;

        public IndexModel()
        {
            _userService = new UserService();
        }

        public string UserName { get; set; }
        public DataTable EnrolledTeachers { get; set; }

        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("Name");
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToPage("/Auth/Login");
            }

            var userIDStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIDStr, out int userID))
            {
                EnrolledTeachers = _userService.GetEnrolledTeachers(userID);
            }

            return Page();
        }
    }
}
