using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.Data;

namespace OnlineQuizSystem.Pages.Results
{
    public class IndexModel : PageModel
    {
        private readonly ResultService _resultService;

        public IndexModel()
        {
            _resultService = new ResultService();
        }

        public DataTable Results { get; set; }

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