using Microsoft.AspNetCore.Mvc;
using Quiz.Model;
using Quiz.Services;
using System.Diagnostics;


namespace QuizApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizServices _quizService;
        public QuizController(IQuizServices quizService) 
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
        }
        [HttpGet("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Question>> StartQuiz()
        {
            _quizService.StartSessions();
            var questions = _quizService.GetQuestions();
            return Ok(questions);
        }
        [HttpPost("submit-answer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<object> SubmitAnswer([FromBody] AnswerRequest request)
        {
            if(request == null || request.QuestionId <= 0 || request.UserAnswer < 0)
            {
                return BadRequest(new { error = "Dữ liệu không hợp lệ" });
            }
            var (isCorrect, feedback) = _quizService.ValidateAnswer(request.QuestionId, request.UserAnswer); // Gọi dịch vụ để xác thực đáp án
            return Ok(new { IsCorrect = isCorrect, Feedback = feedback });
        }
        /// <summary>
        /// Retrieves the results of the current quiz session.
        /// </summary>
        /// <returns>Quiz results including total correct, time taken, and pass status</returns>
        /// <response code="200">Returns the quiz results</response>
        [HttpGet("results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<(int TotalCorrect, TimeSpan TimeTaken, bool Passed, List<QuizResult> Results)> GetResults()
        {
            var result = _quizService.GetQuizResult(); 
            Debug.WriteLine($"Result in controller - TotalCorrect: {result.TotalCorrect}, TimeTaken: {result.TimeTaken}, Passed: {result.Passed}, Results Count: {result.Results?.Count ?? 0}");
            return Ok(result); 
        }
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        public IActionResult ClearResults()
        {
            _quizService.ClearResults(); 
            return NoContent(); 
        }
    }
    public class AnswerRequest
    {
        public int QuestionId { get; set; }
        public int UserAnswer { get; set; }
    }
}