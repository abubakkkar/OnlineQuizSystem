namespace OnlineQuizSystem.BLL.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public char CorrectOption { get; set; }
        public int DifficultyLevel { get; set; }
        public int TeacherID { get; set; }
    }
}