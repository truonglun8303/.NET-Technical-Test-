using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Quiz.Model
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        [JsonIgnore]
        public int CorrectAnswer { get; set; } 
        [JsonIgnore]
        public List<QuizResult> QuizResults { get; set; }
    }
}
