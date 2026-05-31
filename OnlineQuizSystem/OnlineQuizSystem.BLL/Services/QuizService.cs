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
        public int UserID { get; set; }
        public int CurrentSessionID { get; set; }

        public int StartSession(int userID, int? teacherID = null)
        {
            TeacherID = teacherID;
            CurrentSessionID = quizRepo.StartQuizSession(userID, teacherID);
            return CurrentSessionID;
        }

        public bool HasCompletedQuizForTeacher(int userID, int teacherID)
        {
            return quizRepo.HasCompletedQuizForTeacher(userID, teacherID);
        }

        public DataRow GetQuestion()
        {
            if (TeacherID.HasValue)
            {
                // Get questions from sections the student is enrolled in for this teacher
                var dt = (UserID > 0) 
                    ? questionRepo.GetByTeacherAndStudent(TeacherID.Value, UserID)
                    : questionRepo.GetByTeacher(TeacherID.Value);
                
                // Get all answered question IDs for the current session
                var answeredIds = new System.Collections.Generic.List<int>();
                if (CurrentSessionID > 0)
                {
                    DataTable answeredDt = quizRepo.GetAnsweredQuestionIDs(CurrentSessionID);
                    foreach (DataRow row in answeredDt.Rows)
                    {
                        if (row["QuestionID"] != DBNull.Value)
                        {
                            answeredIds.Add(Convert.ToInt32(row["QuestionID"]));
                        }
                    }
                }
                
                // First, try to find a question at the current difficulty level
                string filter = $"DifficultyLevel = {Difficulty}";
                if (answeredIds.Count > 0)
                {
                    filter += $" AND QuestionID NOT IN ({string.Join(",", answeredIds)})";
                }

                var filtered = dt.Select(filter);
                if (filtered.Length > 0)
                    return filtered[new Random().Next(filtered.Length)];
                
                // If no questions at current difficulty, try other levels
                for (int level = 1; level <= 3; level++)
                {
                    if (level == Difficulty) continue; // Skip the level we already tried
                    string fallbackFilter = $"DifficultyLevel = {level}";
                    if (answeredIds.Count > 0)
                    {
                        fallbackFilter += $" AND QuestionID NOT IN ({string.Join(",", answeredIds)})";
                    }

                    filtered = dt.Select(fallbackFilter);
                    if (filtered.Length > 0)
                        return filtered[new Random().Next(filtered.Length)];
                }
                
                // No questions found at any difficulty level
                return null;
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

        public int GetAttemptedQuestions()
        {
            if (CurrentSessionID > 0)
            {
                return quizRepo.GetAttemptedQuestions(CurrentSessionID);
            }
            return 0;
        }

        public int GetTotalQuestions()
        {
            if (CurrentSessionID > 0)
            {
                return quizRepo.GetTotalQuestions(CurrentSessionID);
            }
            return 0;
        }

        public void SubmitQuiz()
        {
            if (CurrentSessionID > 0)
            {
                quizRepo.SubmitQuiz(CurrentSessionID);
            }
        }

        public bool IsQuizSubmitted()
        {
            if (CurrentSessionID > 0)
            {
                return quizRepo.IsQuizSubmitted(CurrentSessionID);
            }
            return false;
        }

        public int GetAvailableQuestionCount(int userID, int teacherID)
        {
            return quizRepo.GetAvailableQuestionCount(userID, teacherID);
        }
    }
}