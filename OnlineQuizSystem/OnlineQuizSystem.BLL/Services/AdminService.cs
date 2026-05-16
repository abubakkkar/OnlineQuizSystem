using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class AdminService
    {
        AdminRepository repo = new AdminRepository();

        public bool Login(string email, string pass)
        {
            return repo.Login(email, pass).Rows.Count > 0;
        }

        public DataTable GetAdminByEmail(string email)
        {
            return repo.GetByEmail(email);
        }

        public void Register(string name, string email, string pass)
        {
            repo.Register(name, email, pass);
        }
    }
}
