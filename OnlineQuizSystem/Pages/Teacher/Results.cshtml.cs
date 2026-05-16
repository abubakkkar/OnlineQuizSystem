using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Teacher
{
    public class ResultsModel : PageModel
    {
        private readonly TeacherService _teacherService;

        public ResultsModel()
        {
            _teacherService = new TeacherService();
        }

        public DataTable Results { get; set; }

        public IActionResult OnGet()
        {
            // Check if teacher is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TeacherID")) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToPage("/Auth/Login");
            }

            int teacherID = int.Parse(HttpContext.Session.GetString("TeacherID"));

            Results = _teacherService.GetTeacherResults(teacherID);

            return Page();
        }
    }
}
