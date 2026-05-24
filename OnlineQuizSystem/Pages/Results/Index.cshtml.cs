using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;
using System.Linq;
using System.Collections.Generic;

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
        public int TotalQuizzes { get; set; }
        public int OverallCorrectAnswers { get; set; }
        public int OverallTotalQuestions { get; set; }
        public int OverallPercentage { get; set; }
        public List<QuizSessionSummary> SessionSummaries { get; set; } = new();
        public Dictionary<int, List<DataRow>> SessionResultsBySession { get; set; } = new();

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
                    Results = _resultService.GetResultsByUser(userID);

                    if (Results != null && Results.Rows.Count > 0)
                    {
                        var groups = Results.Rows.Cast<DataRow>()
                            .GroupBy(row => Convert.ToInt32(row["SessionID"]))
                            .OrderByDescending(g => Convert.ToDateTime(g.First()["EndTime"]));

                        SessionSummaries = groups.Select(g =>
                        {
                            var first = g.First();
                            var totalQuestions = Convert.ToInt32(first["TotalQuestions"]);
                            var correctCount = g.Count(row => Convert.ToBoolean(row["IsCorrect"]));

                            return new QuizSessionSummary
                            {
                                SessionID = Convert.ToInt32(first["SessionID"]),
                                QuizTitle = first["QuizTitle"]?.ToString() ?? "Practice Quiz",
                                TotalQuestions = totalQuestions,
                                CorrectCount = correctCount,
                                Percentage = totalQuestions > 0 ? (int)(correctCount * 100 / totalQuestions) : 0,
                                EndTime = Convert.ToDateTime(first["EndTime"]),
                                Score = Convert.ToInt32(first["Score"]),
                                IsSubmitted = Convert.ToBoolean(first["IsSubmitted"])
                            };
                        }).ToList();

                        SessionResultsBySession = groups.ToDictionary(
                            g => Convert.ToInt32(g.Key),
                            g => g.ToList()
                        );

                        OverallCorrectAnswers = SessionSummaries.Sum(s => s.CorrectCount);
                        OverallTotalQuestions = SessionSummaries.Sum(s => s.TotalQuestions);
                        OverallPercentage = OverallTotalQuestions > 0 ? (int)(OverallCorrectAnswers * 100 / OverallTotalQuestions) : 0;
                        TotalQuizzes = SessionSummaries.Count;
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

        public class QuizSessionSummary
        {
            public int SessionID { get; set; }
            public string QuizTitle { get; set; }
            public int TotalQuestions { get; set; }
            public int CorrectCount { get; set; }
            public int Percentage { get; set; }
            public DateTime EndTime { get; set; }
            public int Score { get; set; }
            public bool IsSubmitted { get; set; }
        }
    }
}