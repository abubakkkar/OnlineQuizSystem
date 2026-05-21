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
                // First, check the specific section the student is enrolled in for this teacher
                DataTable maxTimeDt = db.Select($@"
                    SELECT MAX(s.MaxTimeMinutes) as MaxMins 
                    FROM Sections s 
                    INNER JOIN UserSections us ON s.SectionID = us.SectionID 
                    WHERE s.TeacherID = {teacherID.Value} AND us.UserID = {userID}");
                
                if (maxTimeDt.Rows.Count > 0 && maxTimeDt.Rows[0]["MaxMins"] != DBNull.Value)
                {
                    maxMinutes = Convert.ToInt32(maxTimeDt.Rows[0]["MaxMins"]);
                }
                else
                {
                    // Fallback to the maximum section time for this teacher
                    DataTable fallbackDt = db.Select($"SELECT MAX(MaxTimeMinutes) as MaxMins FROM Sections WHERE TeacherID = {teacherID.Value}");
                    if (fallbackDt.Rows.Count > 0 && fallbackDt.Rows[0]["MaxMins"] != DBNull.Value)
                    {
                        maxMinutes = Convert.ToInt32(fallbackDt.Rows[0]["MaxMins"]);
                    }
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

            DataTable dt = db.Select($@"INSERT INTO QuizSessions (UserID, TeacherID, Score, StartTime, EndTime, TotalQuestions, IsSubmitted) 
                VALUES ({userID}, {teacherIDValue}, 0, GETDATE(), DATEADD(minute, {maxMinutes}, GETDATE()), {totalQuestions}, 0); 
                SELECT SCOPE_IDENTITY() as SessionID");
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
            // Calculate score and mark as submitted
            DataTable resultsDt = db.Select($"SELECT COUNT(*) as CorrectCount FROM Results WHERE SessionID = {sessionID} AND IsCorrect = 1");
            int score = 0;
            if (resultsDt.Rows.Count > 0)
            {
                score = Convert.ToInt32(resultsDt.Rows[0]["CorrectCount"]);
            }

            db.Execute($"UPDATE QuizSessions SET Score = {score}, IsSubmitted = 1 WHERE SessionID = {sessionID}");
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