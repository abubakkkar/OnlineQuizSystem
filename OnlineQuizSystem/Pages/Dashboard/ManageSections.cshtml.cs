using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Dashboard
{
    public class ManageSectionsModel : PageModel
    {
        private readonly SectionService _sectionService;

        public ManageSectionsModel()
        {
            _sectionService = new SectionService();
        }

        public DataTable EnrolledSections { get; set; }
        public DataTable AvailableSections { get; set; }
        public string SuccessMessage { get; set; }

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

            LoadSectionData(userID);
            return Page();
        }

        public IActionResult OnPostEnroll(int sectionId)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _sectionService.EnrollUserInSection(userID, sectionId);
                SuccessMessage = "Successfully enrolled in section!";
            }
            catch (Exception ex)
            {
                SuccessMessage = "Failed to enroll: " + ex.Message;
            }

            LoadSectionData(userID);
            return Page();
        }

        public IActionResult OnPostRemove(int sectionId)
        {
            var userIDStr = HttpContext.Session.GetString("UserID");
            if (!int.TryParse(userIDStr, out int userID))
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                _sectionService.RemoveUserFromSection(userID, sectionId);
                SuccessMessage = "Successfully removed from section.";
            }
            catch (Exception ex)
            {
                SuccessMessage = "Failed to remove: " + ex.Message;
            }

            LoadSectionData(userID);
            return Page();
        }

        private void LoadSectionData(int userID)
        {
            EnrolledSections = _sectionService.GetUserSections(userID);
            AvailableSections = _sectionService.GetAvailableSectionsFromEnrolledTeachers(userID);
        }
    }
}
