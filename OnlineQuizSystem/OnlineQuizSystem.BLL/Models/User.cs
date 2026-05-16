namespace OnlineQuizSystem.BLL.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int? TeacherID { get; set; } // Kept for backward compatibility, use Teachers list instead
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}