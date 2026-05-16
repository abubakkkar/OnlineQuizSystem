using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.DAL.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineQuizSystem.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly QuestionRepository _repo;
        private readonly TeacherRepository _teacherRepo;

        public IndexModel()
        {
            _repo = new QuestionRepository();
            _teacherRepo = new TeacherRepository();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string SuccessMessage { get; set; }

        public DataTable Teachers { get; set; }

        public class InputModel
        {
            [Required]
            public string QuestionText { get; set; }

            [Required]
            public string OptionA { get; set; }

            [Required]
            public string OptionB { get; set; }

            [Required]
            public string OptionC { get; set; }

            [Required]
            public string OptionD { get; set; }

            [Required]
            public char CorrectOption { get; set; }

            [Required]
            [Range(1, 3)]
            public int DifficultyLevel { get; set; }

            [Required]
            public int TeacherID { get; set; }
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Admin/Accounts");
        }

        public IActionResult OnPost()
        {
            // Check if admin is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
            {
                return RedirectToPage("/Auth/AdminLogin");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repo.Add(Input.QuestionText, Input.OptionA, Input.OptionB, Input.OptionC, Input.OptionD, Input.CorrectOption, Input.DifficultyLevel, Input.TeacherID);
                    SuccessMessage = "Question added successfully!";
                    return RedirectToPage();
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add question. Please check database connection.");
                }
            }

            Teachers = _teacherRepo.GetAllTeachers();
            return Page();
        }
    }
}
