﻿//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace CSETWebCore.Business.Maturity
{
    public class CpgBusiness
    {
        private readonly CSETContext _context;
        private readonly TranslationOverlay _overlay;
        private readonly string _lang;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public CpgBusiness(CSETContext context, string lang)
        {
            _context = context;

            _overlay = new TranslationOverlay();
            _lang = lang;
        }


        /// <summary>
        /// Returns the answer percentage distributions for each of the CPG domains.
        /// </summary>
        /// <returns></returns>
        public List<AnswerDistribDomain> GetAnswerDistribForDomains(int assessmentId, int? modelId, string techDomain)
        {
            var resp = new List<AnswerDistribDomain>();

            if (modelId == null)
            {
                modelId = _context.AVAILABLE_MATURITY_MODELS.Where(x => x.Assessment_Id == assessmentId).FirstOrDefault()?.model_id;
            }


            // get the CPG question distribution
            var dbListCpg = GetAnswerDistribGroupings(assessmentId, techDomain, modelId);

            foreach (var item in dbListCpg)
            {
                // translate if necessary
                item.title = _overlay.GetMaturityGrouping(item.grouping_id, _lang)?.Title ?? item.title;
                if (!resp.Exists(x => x.Name == item.title))
                {
                    var domain = new AnswerDistribDomain()
                    {
                        Name = item.title,
                        Series = InitializeSeries()
                    };

                    resp.Add(domain);
                }
            }

            // determine percentages for each answer count in the distribution
            resp.ForEach(domain =>
            {
                domain.Series.ForEach(y =>
                {
                    double percent = CalculatePercent(dbListCpg.Where(g => g.title == domain.Name).ToList(), y.Name);
                    y.Value = percent;

                });
            });

            return resp;
        }


        /// <summary>
        /// Calculates the percentage based on all answer values for the domain
        /// </summary>
        /// <returns></returns>
        private double CalculatePercent(List<GetAnswerDistribGroupingsResult> r, string ansName)
        {
            var target = r.FirstOrDefault(x => x.answer_text == ansName)?.answer_count ?? 0;
            var total = r.Select(x => x.answer_count).Sum();

            return ((double)target * 100d / (double)total);
        }


        /// <summary>
        /// Initializes 'empty' percentge slots for potential CPG answers.
        /// </summary>
        /// <returns></returns>
        private List<Series> InitializeSeries()
        {
            var list = new List<Series>();

            var values = new List<string>() { "Y", "I", "S", "N", "U" };
            foreach (string s in values)
            {
                list.Add(new Series()
                {
                    Name = s,
                    Value = 0
                });
            }

            return list;
        }


        /// <summary>
        /// Figures out if an SSG model is applicable as a bonus.
        /// The SSG is based on the assessment's sector.
        /// Returns null if no SSG is applicable.
        /// </summary>
        /// <returns></returns>
        public List<int> DetermineSsgModels(int assessmentId)
        {
            List<int> list = [];

            var ddSectors = _context.DETAILS_DEMOGRAPHICS.Where(x => x.Assessment_Id == assessmentId && x.DataItemName.StartsWith("SSG-SECTOR-")).ToList();

            foreach (var s in ddSectors)
            {
                // CHEMICAL
                var chemicalSectors = new List<int>() { 1, 19 };
                if (chemicalSectors.Contains((int)s.IntValue))
                {
                    list.Add(Constants.Constants.Model_SSG_CHEM);
                }

                // INFORMATION TECHNOLOGY (IT)
                var itSectors = new List<int>() { 13, 28 };
                if (itSectors.Contains((int)s.IntValue))
                {
                    list.Add(Constants.Constants.Model_SSG_IT);
                }
            }

            return list;
        }


        /// <summary>
        /// Returns a list of answer quantities for each domain.  
        /// It considers question scope based on the technical domain
        /// of the assessment. 
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public IList<GetAnswerDistribGroupingsResult> GetAnswerDistribGroupings(int assessmentId, string techDomain, int? modelId)
        {
            _context.FillEmptyMaturityQuestionsForAnalysis(assessmentId);

            if (modelId != null)
            {
                _context.FillEmptyMaturityQuestionsForModel(assessmentId, (int)modelId);
            }


            var resp = new List<GetAnswerDistribGroupingsResult>();


            var query = from a in _context.ANSWER
                        join q in _context.MATURITY_QUESTIONS on a.Question_Or_Requirement_Id equals q.Mat_Question_Id
                        join g in _context.MATURITY_GROUPINGS on q.Grouping_Id equals g.Grouping_Id
                        where a.Question_Type == "Maturity" && q.Is_Answerable
                           && a.Assessment_Id == assessmentId && q.Maturity_Model_Id == modelId
                        select new { g.Grouping_Id, g.Title, a.Answer_Id, a.Answer_Text, q.Mat_Question_Id };

            var answerList = query.ToList();


            // Spin up the generic scope analyzer or a maturity model-specific one
            var _questionScope = new QuestionScopeAnalyzer(assessmentId);


            // CPG 2.0
            if (modelId == Constants.Constants.Model_CPG2)
            {
                _questionScope = new QuestionScopeAnalyzer(assessmentId, _context, techDomain);
            }


            answerList.RemoveAll(x => _questionScope.OutOfScopeQuestionIds.Contains(x.Mat_Question_Id));


            // group the answers 
            var groupedList = answerList
                .GroupBy(o => new { o.Grouping_Id, o.Title, o.Answer_Text })
                .Select(g => new GetAnswerDistribGroupingsResult
                {
                    grouping_id = g.Key.Grouping_Id,
                    title = g.Key.Title,
                    answer_text = g.Key.Answer_Text,
                    answer_count = g.Count()
                })
                .ToList();

            return groupedList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        public double CalculateScore(int assessmentId)
        {
            return 99.9d;
        }
    }
}
