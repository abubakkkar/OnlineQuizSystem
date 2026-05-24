using OnlineQuizSystem.DAL;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class QuizRepository
    {
        DBHelper db = new DBHelper();

        public int StartQuizSession(int userID, int? teacherID = null)
        {
            // Insert new session and return the SessionID in a single batch
            string teacherIDValue = teacherID.HasValue ? teacherID.Value.ToString() : "NULL";
            int maxMinutes = 30;

            if (teacherID.HasValue)
            {
                DataTable sectionTimeDt = db.Select($@"SELECT TOP 1 ISNULL(s.MaxTimeMinutes, 30) AS MaxTimeMinutes
                    FROM Sections s
                    INNER JOIN UserSections us ON s.SectionID = us.SectionID
                    WHERE s.TeacherID = {teacherID.Value}
                    AND us.UserID = {userID}
                    ORDER BY s.MaxTimeMinutes ASC");

                if (sectionTimeDt.Rows.Count > 0 && sectionTimeDt.Rows[0]["MaxTimeMinutes"] != DBNull.Value)
                {
                    maxMinutes = Convert.ToInt32(sectionTimeDt.Rows[0]["MaxTimeMinutes"]);
                }
            }

            // Get total questions for this teacher/user combination
            string getTotalQuery = teacherID.HasValue 
                ? $@"SELECT COUNT(*) as TotalQuestions FROM Questions 
                    WHERE TeacherID = {teacherID.Value}
                    AND (SectionID IS NULL OR SectionID IN (
                        SELECT us.SectionID FROM UserSections us WHERE us.UserID = {userID}
                    ))"
                : $"SELECT COUNT(*) as TotalQuestions FROM Questions";

            DataTable totalDt = db.Select(getTotalQuery);
            int totalQuestions = 0;
            if (totalDt.Rows.Count > 0)
            {
                totalQuestions = Convert.ToInt32(totalDt.Rows[0]["TotalQuestions"]);
            }

            DataTable dt = db.Select($@"EXEC dbo.sp_StartQuizSession @UserID={userID}, @TeacherID={(teacherID.HasValue ? teacherID.Value.ToString() : "NULL")}, @MaxTimeMinutes={maxMinutes}");
            if (dt.Rows.Count > 0 && dt.Rows[0]["SessionID"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["SessionID"]);
            }
            return 0;
        }

        public DataTable GetAnsweredQuestionIDs(int sessionID)
        {
            return db.Select($"SELECT QuestionID FROM Results WHERE SessionID = {sessionID}");
        }

        public bool HasCompletedQuizForTeacher(int userID, int teacherID)
        {
            DataTable dt = db.Select($"SELECT COUNT(*) as CompletedCount FROM QuizSessions WHERE UserID = {userID} AND TeacherID = {teacherID} AND IsSubmitted = 1");
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["CompletedCount"]) > 0;
            }
            return false;
        }

        public void UpdateSessionScore(int sessionID, int score)
        {
            db.Execute($"UPDATE QuizSessions SET Score = {score} WHERE SessionID = {sessionID}");
        }

        public int GetRemainingTimeSeconds(int sessionID)
        {
            DataTable dt = db.Select($"SELECT DATEDIFF(second, GETDATE(), EndTime) as RemainingSeconds FROM QuizSessions WHERE SessionID = {sessionID}");
            if (dt.Rows.Count > 0 && dt.Rows[0]["RemainingSeconds"] != DBNull.Value)
            {
                int remaining = Convert.ToInt32(dt.Rows[0]["RemainingSeconds"]);
                return remaining > 0 ? remaining : 0;
            }
            return 0;
        }

        public int GetAttemptedQuestions(int sessionID)
        {
            DataTable dt = db.Select($"SELECT COUNT(DISTINCT QuestionID) as AttemptedCount FROM Results WHERE SessionID = {sessionID}");
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["AttemptedCount"]);
            }
            return 0;
        }

        public int GetTotalQuestions(int sessionID)
        {
            DataTable dt = db.Select($"SELECT TotalQuestions FROM QuizSessions WHERE SessionID = {sessionID}");
            if (dt.Rows.Count > 0 && dt.Rows[0]["TotalQuestions"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["TotalQuestions"]);
            }
            return 0;
        }

        public void SubmitQuiz(int sessionID)
        {
            db.Execute($"EXEC dbo.sp_SubmitQuiz @SessionID={sessionID}");
        }

        public bool IsQuizSubmitted(int sessionID)
        {
            DataTable dt = db.Select($"SELECT IsSubmitted FROM QuizSessions WHERE SessionID = {sessionID}");
            if (dt.Rows.Count > 0)
            {
                return Convert.ToBoolean(dt.Rows[0]["IsSubmitted"]);
            }
            return false;
        }
    }
}