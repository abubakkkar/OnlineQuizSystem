using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.Pages.Admin
{
    public class ResultsModel : PageModel
    {
        private readonly AdminRepository _adminRepo;

        public ResultsModel()
        {
            _adminRepo = new AdminRepository();
        }

        public DataTable Results { get; set; }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Admin/Accounts");
        }
    }
}
