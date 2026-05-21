using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Teacher
{
    public class ManageSectionsModel : PageModel
    {
        private readonly SectionService _sectionService;

        public ManageSectionsModel()
        {
            _sectionService = new SectionService();
        }

        public DataTable TeacherSections { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                return RedirectToPage("/Auth/Login");
            }

            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/AdminLogin");
            }

            LoadSectionData(teacherID);
            return Page();
        }

        public IActionResult OnPostCreate(string SectionName, string Description, int MaxTimeMinutes = 5)
        {
            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (string.IsNullOrWhiteSpace(SectionName))
            {
                ErrorMessage = "Section name is required.";
                LoadSectionData(teacherID);
                return Page();
            }

            try
            {
                _sectionService.CreateSection(teacherID, SectionName, Description ?? "", MaxTimeMinutes);
                SuccessMessage = "Section created successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to create section: " + ex.Message;
            }

            LoadSectionData(teacherID);
            return Page();
        }

        public IActionResult OnPostDelete(int sectionId)
        {
            var teacherIDStr = HttpContext.Session.GetString("TeacherID");
            if (!int.TryParse(teacherIDStr, out int teacherID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _sectionService.DeleteSection(sectionId);
                SuccessMessage = "Section deleted successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to delete section: " + ex.Message;
            }

            LoadSectionData(teacherID);
            return Page();
        }

        private void LoadSectionData(int teacherID)
        {
            TeacherSections = _sectionService.GetTeacherSections(teacherID);
        }
    }
}
