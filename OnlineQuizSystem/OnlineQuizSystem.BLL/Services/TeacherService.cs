using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class TeacherService
    {
        TeacherRepository repo = new TeacherRepository();

        public DataTable Login(string email, string pass)
        {
            return repo.Login(email, pass);
        }

        public void Register(string n, string e, string p)
        {
            repo.Register(n, e, p);
        }

        public DataTable GetAllTeachers()
        {
            return repo.GetAllTeachers();
        }

        public DataTable GetTeacherByEmail(string email)
        {
            return repo.GetByEmail(email);
        }

        public DataTable GetTeacherResults(int teacherId)
        {
            return repo.GetTeacherResults(teacherId);
        }

        public void AutoSubmitExpiredSessions()
        {
            var quizRepo = new OnlineQuizSystem.DAL.Repositories.QuizRepository();
            quizRepo.AutoSubmitExpiredSessions();
        }
    }
}