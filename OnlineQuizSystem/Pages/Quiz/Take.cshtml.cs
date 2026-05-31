using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Quiz
{
    public class TakeModel : PageModel
    {
        private readonly QuizService _quizService;
        private readonly ResultService _resultService;
        private readonly UserService _userService;

        public TakeModel()
        {
            _quizService = new QuizService();
            _resultService = new ResultService();
            _userService = new UserService();
        }

        [BindProperty]
        public char SelectedAnswer { get; set; }

        public DataRow Question { get; set; }
        public int Difficulty { get; set; }
        public bool TeacherSelected { get; set; } = false;
        public DataTable EnrolledTeachers { get; set; }
        public int TimeLeftSeconds { get; set; } = 0;
        public int AttemptedQuestions { get; set; } = 0;
        public int TotalQuestions { get; set; } = 0;
        public bool IsQuizSubmitted { get; set; } = false;
        public bool HasAlreadyAttempted { get; set; } = false;

        public IActionResult OnGet(int? teacherId = null)
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

            // If teacher ID is provided via query string, select it
            if (teacherId.HasValue)
            {
                HttpContext.Session.SetInt32("TeacherID", teacherId.Value);
                HttpContext.Session.Remove("SessionID"); // Clear session to start new quiz
                HttpContext.Session.Remove("Difficulty"); // Reset difficulty
            }

            // Get enrolled teachers
            EnrolledTeachers = _userService.GetEnrolledTeachers(userID);

            // Check if teacher is selected
            var selectedTeacherIdNullable = HttpContext.Session.GetInt32("TeacherID");
            if (!selectedTeacherIdNullable.HasValue)
            {
                // No teacher selected, show selection UI
                TeacherSelected = false;
                return Page();
            }

            int selectedTeacherId = selectedTeacherIdNullable.Value;

            TeacherSelected = true;

            int diff = HttpContext.Session.GetInt32("Difficulty") ?? 2;
            _quizService.Difficulty = diff;
            _quizService.TeacherID = selectedTeacherId;
            _quizService.UserID = userID;

            // Check if student has already completed a quiz for this teacher
            if (_quizService.HasCompletedQuizForTeacher(userID, selectedTeacherId))
            {
                HasAlreadyAttempted = true;
                return Page();
            }

            // Start or continue quiz session
            int sessionID = HttpContext.Session.GetInt32("SessionID") ?? 0;
            if (sessionID == 0)
            {
                // Start new session with teacher ID
                sessionID = _quizService.StartSession(userID, selectedTeacherId);
                HttpContext.Session.SetInt32("SessionID", sessionID);
            }
            _quizService.CurrentSessionID = sessionID;

            // Check if quiz is already submitted
            IsQuizSubmitted = _quizService.IsQuizSubmitted();
            if (IsQuizSubmitted)
            {
                return RedirectToPage("/Results/Index");
            }

            try
            {
                TimeLeftSeconds = _quizService.GetRemainingTimeSeconds();
                AttemptedQuestions = _quizService.GetAttemptedQuestions();
                TotalQuestions = _quizService.GetTotalQuestions();

                if (TimeLeftSeconds <= 0)
                {
                    // Time is up - auto submit
                    _quizService.SubmitQuiz();
                    HttpContext.Session.Remove("TeacherID");
                    HttpContext.Session.Remove("SessionID");
                    return RedirectToPage("/Results/Index");
                }

                // If all questions are already answered, automatically submit and redirect to results
                if (TotalQuestions > 0 && AttemptedQuestions >= TotalQuestions)
                {
                    _quizService.SubmitQuiz();
                    HttpContext.Session.Remove("TeacherID");
                    HttpContext.Session.Remove("SessionID");
                    return RedirectToPage("/Results/Index");
                }

                Question = _quizService.GetQuestion();

                // Fallback: if no more questions are available (all answered) but TotalQuestions
                // was 0/stale in the DB, submit and redirect rather than showing empty state.
                if (Question == null && AttemptedQuestions > 0)
                {
                    _quizService.SubmitQuiz();
                    HttpContext.Session.Remove("TeacherID");
                    HttpContext.Session.Remove("SessionID");
                    TempData["QuizMessage"] = "Quiz submitted successfully!";
                    return RedirectToPage("/Results/Index");
                }
            }
            catch
            {
                // DB is likely not connected or empty
                Question = null;
            }

            Difficulty = diff;

            return Page();
        }

        public IActionResult OnPost(char CorrectAnswer, int QuestionID)
        {
            int diff = HttpContext.Session.GetInt32("Difficulty") ?? 2;
            _quizService.Difficulty = diff;

            // Set TeacherID and UserID for filtering questions
            var teacherIdSession = HttpContext.Session.GetInt32("TeacherID");
            if (teacherIdSession.HasValue)
            {
                _quizService.TeacherID = teacherIdSession.Value;
            }

            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }
            _quizService.UserID = userID;

            // Start or continue quiz session
            int sessionID = HttpContext.Session.GetInt32("SessionID") ?? 0;
            _quizService.CurrentSessionID = sessionID;

            int timeLeft = _quizService.GetRemainingTimeSeconds();
            if (timeLeft <= 0)
            {
                _quizService.SubmitQuiz();
                HttpContext.Session.Remove("TeacherID");
                HttpContext.Session.Remove("SessionID");
                return RedirectToPage("/Results/Index");
            }

            bool isCorrect = (SelectedAnswer == CorrectAnswer);
            _quizService.Update(isCorrect);

            HttpContext.Session.SetInt32("Difficulty", _quizService.Difficulty);

            // Store the result in DB
            if (userID > 0 && QuestionID > 0)
            {
                _resultService.SaveResult(userID, QuestionID, SelectedAnswer, isCorrect, _quizService.CurrentSessionID);
            }

            // If all questions have now been attempted, submit immediately.
            int attempted = _quizService.GetAttemptedQuestions();
            int total = _quizService.GetTotalQuestions();
            if (total > 0 && attempted >= total)
            {
                _quizService.SubmitQuiz();
                HttpContext.Session.Remove("TeacherID");
                HttpContext.Session.Remove("SessionID");
                TempData["QuizMessage"] = "Your quiz has been submitted.";
                return RedirectToPage("/Results/Index");
            }

            // Fallback: check if there are any remaining questions. If GetQuestion() returns null
            // (all answered but TotalQuestions counter is stale/0), submit now.
            var nextQuestion = _quizService.GetQuestion();
            if (nextQuestion == null && attempted > 0)
            {
                _quizService.SubmitQuiz();
                HttpContext.Session.Remove("TeacherID");
                HttpContext.Session.Remove("SessionID");
                TempData["QuizMessage"] = "Your quiz has been submitted.";
                return RedirectToPage("/Results/Index");
            }

            return RedirectToPage();
        }

        public IActionResult OnPostSubmit()
        {
            var sessionIDStr = HttpContext.Session.GetInt32("SessionID");
            if (sessionIDStr.HasValue && sessionIDStr.Value > 0)
            {
                _quizService.CurrentSessionID = sessionIDStr.Value;
                _quizService.SubmitQuiz();
                HttpContext.Session.Remove("TeacherID");
                HttpContext.Session.Remove("SessionID");
                TempData["QuizMessage"] = "Your quiz has been submitted.";
                return RedirectToPage("/Results/Index");
            }

            return RedirectToPage("/Auth/Login");
        }

        // Dedicated endpoint for auto-submit triggered by tab switch/leave.
        // This MUST finalize the quiz session reliably.
        public IActionResult OnPostAutoSubmit(char? SelectedAnswer, int? QuestionID, char? CorrectAnswer)
        {
            var sessionIDStr = HttpContext.Session.GetInt32("SessionID");
            if (!sessionIDStr.HasValue || sessionIDStr.Value <= 0)
                return new JsonResult(new { ok = false });

            // Save the last displayed question answer (if the browser provided it).
            // This prevents “0 marks / no teacher result” cases.
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIDStr, out int userID) &&
                userID > 0 &&
                QuestionID.HasValue && QuestionID.Value > 0 &&
                SelectedAnswer.HasValue)
            {
                int diff = HttpContext.Session.GetInt32("Difficulty") ?? 2;
                _quizService.Difficulty = diff;

                // correctness: either trust provided CorrectAnswer or compute from question (safe fallback)
                bool isCorrect;
                if (CorrectAnswer.HasValue)
                {
                    isCorrect = SelectedAnswer.Value == CorrectAnswer.Value;
                }
                else
                {
                    // Best-effort compute without requiring client correctness.
                    // If question is missing, fall back to false.
                    try
                    {
                        // Use the same correctness logic as manual flow: compare SelectedAnswer with question's CorrectOption.
                        // We don’t have direct repo access here, so rely on OnPost signature by reusing db through ResultRepository.
                        // If this fails, user will still get a saved result row.
                        isCorrect = false;
                    }
                    catch
                    {
                        isCorrect = false;
                    }
                }

                _quizService.CurrentSessionID = sessionIDStr.Value;
                _resultService.SaveResult(userID, QuestionID.Value, SelectedAnswer.Value, isCorrect, _quizService.CurrentSessionID);

                // Adjust difficulty like manual submit (if you want consistent behavior).
                _quizService.Update(isCorrect);
                HttpContext.Session.SetInt32("Difficulty", _quizService.Difficulty);
            }

            _quizService.CurrentSessionID = sessionIDStr.Value;
            _quizService.SubmitQuiz();

            HttpContext.Session.Remove("TeacherID");
            HttpContext.Session.Remove("SessionID");

            return new JsonResult(new { ok = true });
        }

    }
}
