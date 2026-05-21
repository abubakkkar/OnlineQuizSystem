using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineQuizSystem.Pages.Auth
{
    public class AdminLoginModel : PageModel
    {
        private readonly AdminService _adminService;

        public AdminLoginModel()
        {
            _adminService = new AdminService();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (_adminService.Login(Input.Email, Input.Password))
                {
                    var adminData = _adminService.GetAdminByEmail(Input.Email);
                    if (adminData.Rows.Count > 0)
                    {
                        var adminRow = adminData.Rows[0];
                        HttpContext.Session.SetString("AdminName", adminRow["Name"].ToString());
                        HttpContext.Session.SetString("AdminRole", adminRow["Role"].ToString());
                        HttpContext.Session.SetString("AdminEmail", adminRow["Email"].ToString());
                        HttpContext.Session.SetString("AdminID", adminRow["AdminID"].ToString());
                        
                        return RedirectToPage("/Admin/Index");
                    }
                }
                
                ErrorMessage = "Invalid email or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login. Please try again. Details: {ex.Message}";
            }

            return Page();
        }
    }
}
