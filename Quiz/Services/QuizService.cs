using Quiz.Model;
using Quiz.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quiz.Services
{
    public class QuizService : IQuizServices
    {
        private readonly List<Question> questions = new List<Question>
        {
            new Question { Id = 1, QuestionText = "What is 2 + 2?", Options = new List<string> { "3", "4", "5", "6" }, CorrectAnswer = 1 },
            new Question { Id = 2, QuestionText = "Capital of France?", Options = new List<string> { "London", "Berlin", "Paris", "Madrid" }, CorrectAnswer = 2 },
            new Question { Id = 3, QuestionText = "What color is the sky?", Options = new List<string> { "Green", "Blue", "Red", "Yellow" }, CorrectAnswer = 1 }
        };

        private List<QuizResult> results = new List<QuizResult>();
        private DateTime? startTime;

        public void StartSessions()
        {
            ClearResults();
            startTime = DateTime.Now;
        }

        public List<Question> GetQuestions()
        {
            return questions;
        }

        public (bool IsCorrect, string Feedback) ValidateAnswer(int questionId, int userAnswer)
        {
            if (!IsSessionActive()) return (false, "No active session. Please start a new quiz.");
            var question = questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return (false, "Question not found");

            bool isCorrect = userAnswer == question.CorrectAnswer;
            string feedback = isCorrect ? "Correct!" : $"Wrong. Correct answer is {question.Options[question.CorrectAnswer]}";
            results.Add(new QuizResult { QuestionId = questionId, UserAnswer = userAnswer, IsCorrect = isCorrect, Feedback = feedback });
            return (isCorrect, feedback);
        }

        public (int TotalCorrect, TimeSpan TimeTaken, bool Passed, List<QuizResult> Results) GetQuizResult()
        {
            if (!IsSessionActive()) return (0, TimeSpan.Zero, false, new List<QuizResult>());
            int totalCorrect = results.Count(r => r.IsCorrect);
            TimeSpan timeTaken = DateTime.Now - startTime.Value;
            bool passed = totalCorrect >= questions.Count / 2;
            return (totalCorrect, timeTaken, passed, results);
        }

        public void ClearResults()
        {
            results.Clear();
            startTime = null;
        }

        public bool IsSessionActive()
        {
            return startTime.HasValue;
        }
    }
}