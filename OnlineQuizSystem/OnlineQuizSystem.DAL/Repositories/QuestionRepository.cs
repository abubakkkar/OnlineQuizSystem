using OnlineQuizSystem.DAL;
using System;
using System.Data;

namespace OnlineQuizSystem.DAL.Repositories
{
    public class QuestionRepository
    {
        DBHelper db = new DBHelper();

        public DataTable GetByDifficulty(int level)
        {
            return db.Select($"SELECT * FROM Questions WHERE DifficultyLevel={level}");
        }

        public DataTable GetByTeacher(int teacherID)
        {
            return db.Select($"SELECT * FROM Questions WHERE TeacherID={teacherID}");
        }

        // Get questions from sections the user is enrolled in for a specific teacher
        public DataTable GetByTeacherAndStudent(int teacherID, int userID)
        {
            string query = $@"
                SELECT DISTINCT q.* FROM Questions q
                WHERE q.TeacherID = {teacherID}
                AND (
                    q.SectionID IS NULL 
                    OR q.SectionID IN (
                        SELECT us.SectionID 
                        FROM UserSections us 
                        WHERE us.UserID = {userID}
                    )
                )
                ORDER BY q.QuestionID";
            
            return db.Select(query);
        }

        public DataRow GetRandom(int level)
        {
            var dt = GetByDifficulty(level);
            if (dt.Rows.Count == 0) return null;

            return dt.Rows[new Random().Next(dt.Rows.Count)];
        }

        public void Add(string q, string a, string b, string c, string d, char correct, int level, int teacherID, int? sectionId = null)
        {
            string sectionVal = sectionId.HasValue && sectionId.Value > 0 ? sectionId.Value.ToString() : "NULL";
            db.Execute($"INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, DifficultyLevel, TeacherID, SectionID) VALUES('{q.Replace("'", "''")}','{a.Replace("'", "''")}','{b.Replace("'", "''")}','{c.Replace("'", "''")}','{d.Replace("'", "''")}','{correct}',{level},{teacherID}, {sectionVal})");
        }
    }
}