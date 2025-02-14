using ProductClassification.Models;
using System.Text.Json;

namespace ProductClassification.Data
{
    public static class DBInitializer
    {
        public static void Initialize(ApplicationDBContext dbContext)
        {
            try
            {
                dbContext.Database.EnsureCreated();

                if (dbContext.EvaluationData.Any())
                {
                    return;
                }

                // Read the Json File 
                string jsonfilepath = Path.Combine(AppContext.BaseDirectory,Path.Join("Data","Evaluation.json"));
                string json = File.ReadAllText(jsonfilepath);

                EvaluationData[] evaldata = JsonSerializer.Deserialize<EvaluationListModel>(json).Evaluate.Select((data) => new EvaluationData() { Answer = data.Answer, Description = data.Description, Reason = data.Reason }).ToArray();

                dbContext.EvaluationData.AddRange(evaldata);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw;
            }



        }
    }
}
