namespace OnlineQuizSystem.BLL.Models
{
    public class Result
    {
        public int ResultID { get; set; }
        public int UserID { get; set; }
        public int QuestionID { get; set; }
        public char SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; }
        public int SessionID { get; set; }
    }
}