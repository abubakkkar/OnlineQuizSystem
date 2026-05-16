using System.Data;
using OnlineQuizSystem.DAL.Repositories;

namespace OnlineQuizSystem.BLL.Services
{
    public class QuizService
    {
        QuestionRepository questionRepo = new QuestionRepository();
        QuizRepository quizRepo = new QuizRepository();

        public int Difficulty = 2;
        public int? TeacherID { get; set; }
        public int CurrentSessionID { get; set; }

        public int StartSession(int userID, int? teacherID = null)
        {
            TeacherID = teacherID;
            CurrentSessionID = quizRepo.StartQuizSession(userID, teacherID);
            return CurrentSessionID;
        }

        public DataRow GetQuestion()
        {
            if (TeacherID.HasValue)
            {
                var dt = questionRepo.GetByTeacher(TeacherID.Value);
                // Filter by difficulty
                var filtered = dt.Select($"DifficultyLevel = {Difficulty}");
                if (filtered.Length == 0) return null;
                return filtered[new Random().Next(filtered.Length)];
            }
            else
            {
                return questionRepo.GetRandom(Difficulty);
            }
        }

        public void Update(bool correct)
        {
            if (correct) Difficulty++;
            else Difficulty--;

            if (Difficulty < 1) Difficulty = 1;
            if (Difficulty > 3) Difficulty = 3;
        }

        public int GetRemainingTimeSeconds()
        {
            if (CurrentSessionID > 0)
            {
                return quizRepo.GetRemainingTimeSeconds(CurrentSessionID);
            }
            return 0;
        }
    }
}