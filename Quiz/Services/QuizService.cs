using Quiz.Data;
using Quiz.Model;
namespace Quiz.Services
{
    public class QuizService : IQuizServices
    {
        private readonly QuizDbContext _context;
        private DateTime? startTime;
        private readonly IHttpContextAccessor _httpContextAccessor;
                
        public QuizService(QuizDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void StartSessions()
        {
            ClearResults();
            _httpContextAccessor.HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());
        }

        public List<Question> GetQuestions()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("QuizDbContext chưa được khởi tạo.");
            }
            try
            {
                var questions = _context.Questions.ToList();
                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi truy vấn Questions: {ex.Message}");
                throw; 
            }
        }
        
        public (bool IsCorrect, string Feedback) ValidateAnswer(int questionId, int userAnswer)
        {
            // Check if _context has been initialized
            if (_context == null)
            {
                throw new InvalidOperationException("QuizDbContext chưa được khởi tạo.");
            }
            // Check session before processing the answer
            if (!IsSessionActive())
            {
                return (false, "Phiên không hoạt động, vui lòng bắt đầu phiên mới.");
            }
            
            var question = _context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                return (false, "Không tìm thấy câu hỏi");
            }
            // Compare the user's answer with the correct answer
            bool isCorrect = userAnswer == question.CorrectAnswer;
            string feedback = isCorrect ? "Correct!" : $"Wrong. Correct answer is {question.Options[question.CorrectAnswer]}";
            // Save the result to the database
            var result = new QuizResult
            {
                QuestionId = questionId,
                UserAnswer = userAnswer,
                IsCorrect = isCorrect,
                Feedback = feedback
            };
            _context.QuizResults.Add(result);
            _context.SaveChanges();
            return (isCorrect, feedback);
        }

        public (int TotalCorrect, TimeSpan TimeTaken, bool Passed, List<QuizResult> Results) GetQuizResult()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("QuizDbContext is not initialized.");
            }
            if (!IsSessionActive())
            {
                return (0, TimeSpan.Zero, false, new List<QuizResult>());
            }
            // Retrieve the start time from the session
            var startTimeString = _httpContextAccessor.HttpContext.Session.GetString("StartTime");
            DateTime startTime;

            if (string.IsNullOrEmpty(startTimeString) || !DateTime.TryParse(startTimeString, out startTime))
            {
                // If parsing fails, consider the session invalid
                ClearResults(); // Reset session
                return (0, TimeSpan.Zero, false, new List<QuizResult>());
            }
            // Retrieve quiz results from the database and calculate the number of correct answers
            var results = _context.QuizResults.ToList();
            int totalCorrect = results.Count(r => r.IsCorrect);
            TimeSpan timeTaken = DateTime.Now - startTime;
            // Evaluate pass/fail: pass if at least 50% of the questions are correct
            bool passed = totalCorrect >= _context.Questions.Count() / 2;

            return (totalCorrect, timeTaken, passed, results);
        }

        public void ClearResults()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("QuizDbContext is not initialized.");
            }
            var resultsToRemove = _context.QuizResults.ToList();
            _context.QuizResults.RemoveRange(resultsToRemove);
            _context.SaveChanges();
            _httpContextAccessor.HttpContext.Session.Remove("StartTime");
        }
        public bool IsSessionActive()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("StartTime") != null;
        }
    }
}