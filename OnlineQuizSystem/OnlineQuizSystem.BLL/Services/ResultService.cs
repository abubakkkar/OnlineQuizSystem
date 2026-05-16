using OnlineQuizSystem.DAL.Repositories;
using System.Data;

namespace OnlineQuizSystem.BLL.Services
{
    public class ResultService
    {
        ResultRepository repo = new ResultRepository();

        public void SaveResult(int userID, int questionID, char selectedAnswer, bool isCorrect, int sessionID)
        {
            repo.SaveResult(userID, questionID, selectedAnswer, isCorrect, sessionID);
        }

        public DataTable GetResultsByUser(int userID)
        {
            return repo.GetResultsByUser(userID);
        }

        public DataTable GetResultsBySession(int sessionID)
        {
            return repo.GetResultsBySession(sessionID);
        }
    }
}