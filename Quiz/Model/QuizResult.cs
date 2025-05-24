using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Quiz.Model
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")] 
        public Question Question { get; set; }
        public int UserAnswer { get; set; } 
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
    }
}
