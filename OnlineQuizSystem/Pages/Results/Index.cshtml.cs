using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Results
{
    public class IndexModel : PageModel
    {
        private readonly ResultService _resultService;
        private readonly QuizService _quizService;

        public IndexModel()
        {
            _resultService = new ResultService();
            _quizService = new QuizService();
        }

        public DataTable Results { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public string TeacherName { get; set; }

        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("Name");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToPage("/Auth/Login");
            }

            var userIDStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIDStr, out int userID))
            {
                try
                {
                    // Get all results for the user
                    Results = _resultService.GetResultsByUser(userID);
                    
                    if (Results != null && Results.Rows.Count > 0)
                    {
                        TotalQuestions = Results.Rows.Count;
                        CorrectAnswers = Results.Rows.Cast<DataRow>()
                            .Count(r => Convert.ToBoolean(r["IsCorrect"]));
                    }
                }
                catch
                {
                    // DB connection issues
                    Results = null;
                }
            }

            return Page();
        }
    }
}