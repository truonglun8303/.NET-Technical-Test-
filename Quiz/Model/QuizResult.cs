namespace Quiz.Model
{
    public class QuizResult
    {
        public int QuestionId { get; set; }
        public int UserAnswer { get; set; } // Index của đáp án người dùng (0-3)
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
    }
}
