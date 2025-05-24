using Quiz.Model;

namespace Quiz.Services
{
    public interface IQuizServices
    {
        void StartSessions();
        List<Question> GetQuestions();
        (bool IsCorrect, string Feedback) ValidateAnswer(int questionId, int userAnser);
        (int TotalCorrect, TimeSpan TimeTaken, bool Passed, List<QuizResult> Results) GetQuizResult();
        void ClearResults();
        bool IsSessionActive();
    }
}
