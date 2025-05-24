using Microsoft.EntityFrameworkCore;
using Quiz.Model;
namespace Quiz.Data
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options):base(options){ }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
    }
}
