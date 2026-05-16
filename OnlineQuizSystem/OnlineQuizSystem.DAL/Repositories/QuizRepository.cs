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
                DataTable maxTimeDt = db.Select($"SELECT MAX(MaxTimeMinutes) as MaxMins FROM Sections WHERE TeacherID = {teacherID.Value}");
                if (maxTimeDt.Rows.Count > 0 && maxTimeDt.Rows[0]["MaxMins"] != DBNull.Value)
                {
                    maxMinutes = Convert.ToInt32(maxTimeDt.Rows[0]["MaxMins"]);
                }
            }

            DataTable dt = db.Select($"INSERT INTO QuizSessions (UserID, TeacherID, Score, StartTime, EndTime) VALUES ({userID}, {teacherIDValue}, 0, GETDATE(), DATEADD(minute, {maxMinutes}, GETDATE())); SELECT SCOPE_IDENTITY() as SessionID");
            if (dt.Rows.Count > 0 && dt.Rows[0]["SessionID"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["SessionID"]);
            }
            return 0;
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
    }
}