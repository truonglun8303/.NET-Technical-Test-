using Microsoft.AspNetCore.Mvc;
using Quiz.Model;

using Quiz.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Starts a new quiz session and returns all available questions.
        /// </summary>
        /// <returns>A list of questions</returns>
        /// <response code="200">Returns the list of questions</response>
        [HttpGet("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Question>> StartQuiz()
        {
            _quizService.StartSessions();
            var questions = _quizService.GetQuestions();
            return Ok(questions);
        }

        /// <summary>
        /// Submits an answer for a specific question and validates it.
        /// </summary>
        /// <param name="request">The request object containing QuestionId and UserAnswer</param>
        /// <returns>Validation result with correctness and feedback</returns>
        /// <response code="200">Returns the validation result</response>
        /// <response code="400">If the request is invalid or no active session</response>
        [HttpPost("submit-answer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<object> SubmitAnswer([FromBody] AnswerRequest request)
        {
            if (request == null || request.UserAnswer < 0 || request.UserAnswer > 3)
            {
                return BadRequest(new { error = "Answer must be between 0 and 3." });
            }

            if (!_quizService.IsSessionActive())
            {
                return BadRequest(new { error = "No active quiz session. Please start a new quiz." });
            }

            var (isCorrect, feedback) = _quizService.ValidateAnswer(request.QuestionId, request.UserAnswer);
            return Ok(new { isCorrect, feedback });
        }

        /// <summary>
        /// Retrieves the results of the current quiz session.
        /// </summary>
        /// <returns>Quiz results including total correct, time taken, and pass status</returns>
        /// <response code="200">Returns the quiz results</response>
        [HttpGet("results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetResults()
        {
            if (!_quizService.IsSessionActive())
            {
                return Ok(new { totalCorrect = 0, totalQuestions = 0, timeTakenSeconds = 0.0, passed = false, results = new List<QuizResult>() });
            }

            var (totalCorrect, timeTaken, passed, allResults) = _quizService.GetQuizResult();
            return Ok(new
            {
                totalCorrect,
                totalQuestions = _quizService.GetQuestions().Count,
                timeTakenSeconds = timeTaken.TotalSeconds,
                passed,
                results = allResults
            });
        }

        /// <summary>
        /// Retrieves a list of incorrectly answered questions from the current session.
        /// </summary>
        /// <returns>A list of wrong answers with details</returns>
        /// <response code="200">Returns the list of wrong answers</response>
        [HttpGet("wrong-answers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<object>> GetWrongAnswers()
        {
            if (!_quizService.IsSessionActive())
            {
                return Ok(new List<object>());
            }

            var (_, _, _, allResults) = _quizService.GetQuizResult();
            var wrongAnswers = allResults
                .Where(r => !r.IsCorrect)
                .Select(r =>
                {
                    var question = _quizService.GetQuestions().First(q => q.Id == r.QuestionId);
                    return new
                    {
                        questionId = r.QuestionId,
                        questionText = question.QuestionText,
                        userAnswer = question.Options[r.UserAnswer],
                        correctAnswer = question.Options[question.CorrectAnswer]
                    };
                })
                .ToList();

            return Ok(wrongAnswers);
        }
    }

    public class AnswerRequest
    {
        public int QuestionId { get; set; }
        public int UserAnswer { get; set; }
    }
}