namespace Quiz.Model
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswer { get; set; } // Index của đáp án đúng (0-3)

        public Question()
        {
            Options = new List<string>();
        }
    }
}
