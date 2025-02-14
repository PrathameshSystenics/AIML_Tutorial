using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductClassification.Models;

namespace ProductClassification.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<EvaluationData> EvaluationData { get; set; }
        public DbSet<EvaluationBatch> EvaluationBatch { get; set; }
        public DbSet<EvaluatedResult> EvaluatedResult { get; set; }
        public DbSet<PromptData> PromptData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
