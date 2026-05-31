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
            // Return all quiz sessions for the user and any associated result rows (if present).
            // This ensures sessions with zero answered questions are still returned so they
            // can be shown with 0 score/0 answers.
              return db.Select($@"SELECT qs.SessionID, qs.UserID, qs.Score,
                             -- Compute total questions for the session dynamically to avoid stale/miscounted values
                             CASE WHEN qs.QuizID IS NOT NULL THEN ISNULL((SELECT COUNT(*) FROM QuizQuestions qq WHERE qq.QuizID = qs.QuizID), 0)
                                 ELSE ISNULL((SELECT COUNT(*) FROM Questions q WHERE (qs.TeacherID IS NULL OR q.TeacherID = qs.TeacherID) AND (q.SectionID IS NULL OR q.SectionID IN (SELECT us.SectionID FROM UserSections us WHERE us.UserID = qs.UserID))), 0)
                             END AS TotalQuestions,
                             qs.StartTime, qs.EndTime, qs.IsSubmitted,
                               COALESCE(qu.Title, t.Name, 'Practice Quiz') AS QuizTitle,
                               r.ResultID, r.QuestionID, r.SelectedAnswer, r.IsCorrect, r.AnsweredAt,
                               q.QuestionText, q.CorrectOption
                        FROM QuizSessions qs
                        LEFT JOIN Results r ON qs.SessionID = r.SessionID
                        LEFT JOIN Questions q ON r.QuestionID = q.QuestionID
                        LEFT JOIN Quizzes qu ON qs.QuizID = qu.QuizID
                        LEFT JOIN Teachers t ON qs.TeacherID = t.TeacherID
                        WHERE qs.UserID = {userID}
                        ORDER BY qs.EndTime DESC, r.AnsweredAt DESC");
        }

        public DataTable GetResultsBySession(int sessionID)
        {
            return db.Select($"SELECT r.*, q.QuestionText, q.CorrectOption FROM Results r INNER JOIN Questions q ON r.QuestionID = q.QuestionID WHERE r.SessionID = {sessionID} ORDER BY r.AnsweredAt");
        }
    }
}