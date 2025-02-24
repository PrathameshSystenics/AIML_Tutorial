using Microsoft.EntityFrameworkCore;
using ProductClassification.Models;

namespace ProductClassification.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<EvaluationData> EvaluationData { get; set; }
        public DbSet<EvaluationBatch> EvaluationBatch { get; set; }
        public DbSet<EvaluatedResult> EvaluatedResult { get; set; }
        public DbSet<PromptData> PromptData { get; set; }

    }
}
