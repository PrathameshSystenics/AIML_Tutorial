using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductClassification.Models;

namespace ProductClassification.Data
{
    public class EvaluationDataRepository
    {
        private readonly ApplicationDBContext _dbcontext;

        public EvaluationDataRepository(ApplicationDBContext dbcontext)
        {
            this._dbcontext = dbcontext;
        }

        public List<EvaluationData> GetEvalQuestions()
        {
            return _dbcontext.EvaluationData.ToList();
        }

        public EvaluationBatch AddNewEvaluationBatch(string modelname, int batchid)
        {
            EntityEntry<EvaluationBatch> newbatchentry = _dbcontext.EvaluationBatch.Add(new EvaluationBatch() { CreatedAt = DateTime.Now, ModelName = modelname, ID = batchid });
            _dbcontext.SaveChanges();
            return newbatchentry.Entity;
        }

        public int GetNewEvaluationBatchID()
        {
            int lastbatchid = 1;
            if (_dbcontext.EvaluationBatch.Any())
            {
                lastbatchid = _dbcontext.EvaluationBatch.Max(eval => eval.ID) + 1;
            }
            return lastbatchid;
        }

        public void AddEvaluationResult(string result, int batchid, int evaldataid, bool isCorrect)
        {
            _dbcontext.EvaluatedResult.Add(new EvaluatedResult()
            {
                CreatedAt = DateTime.Now,
                EvaluationBatchID = batchid,
                EvaluationDataID = evaldataid,
                IsCorrect = isCorrect,
                Result = result,
            });
            _dbcontext.SaveChanges();
        }


        public void AddSystemPromptForBatch(int batchid, string prompt)
        {
            _dbcontext.PromptData.Add(new PromptData()
            {
                EvaluationBatchID = batchid,
                SystemPrompt = prompt
            });
            _dbcontext.SaveChanges();
        }

        public async Task AddEvaluationResultsAsync(List<EvaluatedResult> evalresultlist)
        {
            await _dbcontext.EvaluatedResult.AddRangeAsync(evalresultlist);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<List<EvaluationBatch>> GetEvaluationBatchesWithMetrics()
        {
            IQueryable<EvaluationBatch> query = from batch in _dbcontext.EvaluationBatch
                                                join evalres in _dbcontext.EvaluatedResult
                                                on batch.ID equals evalres.EvaluationBatchID into batchresults
                                                orderby batch.CreatedAt descending
                                                select new EvaluationBatch()
                                                {
                                                    CreatedAt = batch.CreatedAt,
                                                    ID = batch.ID,
                                                    ModelName = batch.ModelName,
                                                    Metrics = new EvaluationMetrics()
                                                    {
                                                        Correct = batchresults.Where(res => res.IsCorrect).Count(),
                                                        Total = batchresults.Count(),
                                                    }
                                                };
            return await query.ToListAsync();
        }

        public EvaluationBatch? GetEvaluationResultsByBatch(int batchID)
        {
            var query = (from eb in _dbcontext.EvaluationBatch
                         where eb.ID == batchID
                         select new EvaluationBatch
                         {
                             ID = eb.ID,
                             CreatedAt = eb.CreatedAt,
                             ModelName = eb.ModelName,
                             EvaluatedResults = (from er in eb.EvaluatedResults
                                                 orderby er.EvaluationDataID
                                                 select new EvaluatedResult
                                                 {
                                                     ID = er.ID,
                                                     EvaluationBatchID = er.EvaluationBatchID,
                                                     EvaluationDataID = er.EvaluationDataID,
                                                     CreatedAt = er.CreatedAt,
                                                     Result = er.Result,
                                                     IsCorrect = er.IsCorrect,
                                                     EvaluationData = new EvaluationData
                                                     {
                                                         ID = er.EvaluationData.ID,
                                                         Description = er.EvaluationData.Description,
                                                         Answer = er.EvaluationData.Answer,
                                                         Reason = er.EvaluationData.Reason,
                                                     }
                                                 }).ToList()
,
                             Metrics = new EvaluationMetrics()
                             {
                                 Correct = eb.EvaluatedResults.Where(res => res.IsCorrect).Count(),
                                 Total = eb.EvaluatedResults.Count()
                             },
                             PromptData = (from prompt in _dbcontext.PromptData
                                           where prompt.EvaluationBatchID == eb.ID
                                           select prompt).First()
                         }).FirstOrDefault();

            return query;
        }
    }
}
