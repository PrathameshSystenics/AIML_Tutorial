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
            EvaluationBatch lastbatch = _dbcontext.EvaluationBatch.OrderBy(batch => batch.ID).LastOrDefault();
            if (lastbatch == null)
            {
                return 1;
            }
            return lastbatch.ID + 1;
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


        public async Task AddEvaluationResultAsync(List<EvaluatedResult> evalresultlist)
        {
            await _dbcontext.EvaluatedResult.AddRangeAsync(evalresultlist);
            await _dbcontext.SaveChangesAsync();
        }




    }
}
