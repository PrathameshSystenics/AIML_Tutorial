using Microsoft.SemanticKernel;
using ProductClassification.Models;
using ProductClassification.Data;
using ProductClassification.SemanticKernel;

namespace ProductClassification.Services
{
    public class EvaluationService
    {
        private ILogger<EvaluationService> _logger;
        private readonly ClassificationService _classificationservice;
        private readonly EvaluationDataRepository _evaluationdatarepo;

        public EvaluationService(ILogger<EvaluationService> logger, ClassificationService classificationservice, EvaluationDataRepository evalrepo)
        {
            _logger = logger;
            _classificationservice = classificationservice;
            _evaluationdatarepo = evalrepo;
        }

        /*public async IAsyncEnumerable<EvaluatedResult> RunCategoryEvaluationBatch(ModelEnum selectedmodel)
        {
            // Add New Evaluation Batch
            int evalbatchid = this._evaluationdatarepo.GetNewEvaluationBatchID();

            // Retreiving the Eval Question
            List<EvaluationData> evaldata = this._evaluationdatarepo.GetEvalQuestions();

            List<EvaluatedResult> evalresultslist = new List<EvaluatedResult>();

            int index = 0;
            bool isEvalCompleted = false;
            int totalquestion = evaldata.Count();
            bool isErrorOccured = false;

            // Looping through each Description
            while (!isEvalCompleted && totalquestion > 0)
            {
                isErrorOccured = false;

                // Get the current evaluation Answer and Description
                EvaluationData current = evaldata[index];

                _logger.LogInformation("Evaluating for Data =>" + current.ID + " With ModelID=" + Enum.GetName<ModelEnum>(selectedmodel));

                ClassificationResult result = new ClassificationResult();
                EvaluatedResult currentevalresult;

                try
                {
                    // classify the result
                    result = await _classificationservice.ClassifyCategoryFromDescription(current.Description, selectedmodel);

                    isEvalCompleted = result.ResultStatus == StatusEnum.Error ? true : false;
                }
                catch (Exception ex)
                {
                    isErrorOccured = true;
                    _logger.LogError(ex.Message + "\nType => " + ex.GetType().ToString());
                }
                finally
                {
                    index++;

                    // if all eval questions are completed then stop the loop
                    if (totalquestion == index)
                    {
                        isEvalCompleted = true;

                    }
                }

                if (!isErrorOccured)
                {
                    currentevalresult = new EvaluatedResult()
                    {
                        CreatedAt = DateTime.Now,
                        EvaluationData = current,
                        EvaluationBatchID = evalbatchid,
                        Result = result.Content,
                        IsCorrect = CompareResults(result.Content, current.Answer) ? true : false
                    };

                    evalresultslist.Add(currentevalresult);

                    yield return currentevalresult;
                }

            }

            // Insert all the results into the database
            if (totalquestion == index)
            {
                yield return new EvaluatedResult() { EvaluationMetrics = GetMetricsFromEvaluationResults(evalresultslist) };

                _evaluationdatarepo.AddNewEvaluationBatch(Enum.GetName<ModelEnum>(selectedmodel) ?? "", evalbatchid);
                await _evaluationdatarepo.AddEvaluationResultAsync(evalresultslist);

            }
        }
*/

        public async IAsyncEnumerable<EvaluatedResult> RunCategoryEvaluationBatch(ModelEnum selectedmodel)
        {
            // Get new evaluation Batch iD
            int evalbatchid = this._evaluationdatarepo.GetNewEvaluationBatchID();

            // Retrieving the Eval Question
            List<EvaluationData> evaldata = this._evaluationdatarepo.GetEvalQuestions();
            List<EvaluatedResult> evalresultslist = new List<EvaluatedResult>();

            bool isErrorOccured = false;
            foreach (var current in evaldata)
            {
                _logger.LogInformation($"Evaluating for Data => {current.ID} With ModelID={Enum.GetName<ModelEnum>(selectedmodel)}");

                ClassificationResult result = new ClassificationResult();
                EvaluatedResult currentevalresult;
                isErrorOccured = false;
                try
                {
                    // Classify the result
                    result = await _classificationservice.ClassifyCategoryFromDescription(current.Description, selectedmodel);
                }
                catch (Exception ex)
                {
                    isErrorOccured = true;
                    _logger.LogError($"{ex.Message}\nType => {ex.GetType()}");
                }

                if (!isErrorOccured && result.ResultStatus != StatusEnum.Error)
                {
                    currentevalresult = new EvaluatedResult()
                    {
                        CreatedAt = DateTime.Now,
                        EvaluationData = current,
                        EvaluationBatchID = evalbatchid,
                        Result = result.Content,
                        IsCorrect = CompareResults(result.Content, current.Answer)
                    };

                    evalresultslist.Add(currentevalresult);
                    yield return currentevalresult;
                }
                else
                {
                    break; // Exit loop on error
                }
            }

            // Insert all the results into the database
            if (evalresultslist.Count > 0 && !isErrorOccured)
            {

                _evaluationdatarepo.AddNewEvaluationBatch(Enum.GetName<ModelEnum>(selectedmodel) ?? "", evalbatchid);
                await _evaluationdatarepo.AddEvaluationResultAsync(evalresultslist);

                yield return new EvaluatedResult() { EvaluationMetrics = GetMetricsFromEvaluationResults(evalresultslist) };
            }
        }

        public EvaluationMetrics GetMetricsFromEvaluationResults(List<EvaluatedResult> evalresultlist)
        {
            if (evalresultlist.Count == 0)
            {
                throw new ArgumentException("List Must have Some Elements in it");
            }
            int totalcorrect = evalresultlist.Where(evals => evals.IsCorrect).Count();

            return new EvaluationMetrics()
            {
                Correct = totalcorrect,
                Total = evalresultlist.Count,
            };
        }

        public bool CompareResults(string modelresult, string actualresult)
        {
            // Checking the thinking steps if the result has the thinking steps then focus on result for checking the answer.
            if (modelresult.Contains("</think>"))
            {
                string[] splitthinkingandActual = modelresult.Split("</think>");
                modelresult = splitthinkingandActual.Length == 2 ? splitthinkingandActual[1] : modelresult;
            }
            return modelresult.Contains(actualresult);
        }
    }
}
