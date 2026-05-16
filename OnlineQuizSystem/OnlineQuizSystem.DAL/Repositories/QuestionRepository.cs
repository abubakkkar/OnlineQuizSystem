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