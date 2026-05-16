using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using OnlineQuizSystem.DAL.Repositories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;

namespace OnlineQuizSystem.Pages.Teacher
{
    public class IndexModel : PageModel
    {
        private readonly QuestionRepository _repo;
        private readonly SectionService _sectionService;

        public IndexModel()
        {
            _repo = new QuestionRepository();
            _sectionService = new SectionService();
        }

        [BindProperty]
        public string QuestionsJson { get; set; }

        [BindProperty]
        public int? SelectedSectionID { get; set; }

        [BindProperty]
        public string NewSectionName { get; set; }

        [BindProperty]
        public int NewSectionMaxTime { get; set; } = 30;

        [TempData]
        public string SuccessMessage { get; set; }

        public DataTable TeacherSections { get; set; }

        public class QuestionInputModel
        {
            public string QuestionText { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public char CorrectOption { get; set; }
            public int DifficultyLevel { get; set; }
        }

        public IActionResult OnGet()
        {
            // Check if teacher is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TeacherID")) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToPage("/Auth/Login");
            }

            int teacherID = int.Parse(HttpContext.Session.GetString("TeacherID"));
            TeacherSections = _sectionService.GetTeacherSections(teacherID);

            return Page();
        }

        public IActionResult OnPost()
        {
            // Check if teacher is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TeacherID")) || HttpContext.Session.GetString("Role") != "Teacher")
            {
                return RedirectToPage("/Auth/Login");
            }

            int teacherID = int.Parse(HttpContext.Session.GetString("TeacherID"));

            try
            {
                var questions = JsonSerializer.Deserialize<List<QuestionInputModel>>(QuestionsJson);

                if (questions == null || questions.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "You must add at least one question.");
                    TeacherSections = _sectionService.GetTeacherSections(teacherID);
                    return Page();
                }

                int finalSectionId = 0;

                // Create a new section if name is provided
                if (!string.IsNullOrWhiteSpace(NewSectionName))
                {
                    finalSectionId = _sectionService.CreateSection(teacherID, NewSectionName, "Generated Section", NewSectionMaxTime);
                }
                else if (SelectedSectionID.HasValue && SelectedSectionID.Value > 0)
                {
                    finalSectionId = SelectedSectionID.Value;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please select an existing section or create a new one.");
                    TeacherSections = _sectionService.GetTeacherSections(teacherID);
                    return Page();
                }

                // Add all questions to the database
                foreach (var q in questions)
                {
                    _repo.Add(q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectOption, q.DifficultyLevel, teacherID, finalSectionId);
                }

                SuccessMessage = $"Quiz uploaded successfully with {questions.Count} questions!";
                return RedirectToPage();
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to upload quiz. " + ex.Message);
            }

            TeacherSections = _sectionService.GetTeacherSections(teacherID);
            return Page();
        }
    }
}