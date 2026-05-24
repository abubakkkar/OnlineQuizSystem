using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class ResultRepository
    {
        DBHelper db = new DBHelper();

        public void SaveResult(int userID, int questionID, char selectedAnswer, bool isCorrect, int sessionID)
        {
            db.Execute($"EXEC dbo.sp_SaveQuizResult @UserID={userID}, @QuestionID={questionID}, @SelectedAnswer='{selectedAnswer}', @SessionID={sessionID}");
        }

        public DataTable GetResultsByUser(int userID)
        {
            return db.Select($"SELECT r.*, q.QuestionText, q.CorrectOption, qs.QuizID, qs.TotalQuestions, qs.Score, qs.StartTime, qs.EndTime, qs.IsSubmitted, COALESCE(qu.Title, t.Name, 'Practice Quiz') AS QuizTitle FROM Results r INNER JOIN Questions q ON r.QuestionID = q.QuestionID INNER JOIN QuizSessions qs ON r.SessionID = qs.SessionID LEFT JOIN Quizzes qu ON qs.QuizID = qu.QuizID LEFT JOIN Teachers t ON qs.TeacherID = t.TeacherID WHERE r.UserID = {userID} ORDER BY qs.EndTime DESC, r.AnsweredAt DESC");
        }

        public DataTable GetResultsBySession(int sessionID)
        {
            return db.Select($"SELECT r.*, q.QuestionText, q.CorrectOption FROM Results r INNER JOIN Questions q ON r.QuestionID = q.QuestionID WHERE r.SessionID = {sessionID} ORDER BY r.AnsweredAt");
        }
    }
}