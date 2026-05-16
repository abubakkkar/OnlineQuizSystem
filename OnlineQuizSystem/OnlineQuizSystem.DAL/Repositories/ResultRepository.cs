using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class ResultRepository
    {
        DBHelper db = new DBHelper();

        public void SaveResult(int userID, int questionID, char selectedAnswer, bool isCorrect, int sessionID)
        {
            db.Execute($"INSERT INTO Results (UserID, QuestionID, SelectedAnswer, IsCorrect, SessionID) VALUES ({userID}, {questionID}, '{selectedAnswer}', {(isCorrect ? 1 : 0)}, {sessionID})");
        }

        public DataTable GetResultsByUser(int userID)
        {
            return db.Select($"SELECT r.*, q.QuestionText, q.CorrectOption FROM Results r INNER JOIN Questions q ON r.QuestionID = q.QuestionID WHERE r.UserID = {userID} ORDER BY r.AnsweredAt DESC");
        }

        public DataTable GetResultsBySession(int sessionID)
        {
            return db.Select($"SELECT r.*, q.QuestionText, q.CorrectOption FROM Results r INNER JOIN Questions q ON r.QuestionID = q.QuestionID WHERE r.SessionID = {sessionID} ORDER BY r.AnsweredAt");
        }
    }
}