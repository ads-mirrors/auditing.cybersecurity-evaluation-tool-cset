//////////////////////////////// 
// 
//   Copyright 2023 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.Nested;

namespace CSETWebCore.Business.Maturity
{
    public class CisScoring
    {
        /// <summary>
        /// The structured model for a section/category in CIS.
        /// </summary>
        public NestedQuestions QuestionsModel;

        private List<FlatOption> allOptionWeights = new List<FlatOption>();

        private List<Model.Nested.Question> allMinHrDayQuestions = new List<Model.Nested.Question>();


        /// <summary>
        /// Calculates CIS scoring.
        /// </summary>
        public CisScoring(int assessmentId, int sectionId, CSETContext context)
        {
            var section = new NestedStructure(assessmentId, sectionId, context);
            QuestionsModel = section.MyModel;
        }


        /// <summary>
        /// Calculates CIS scoring.
        /// </summary>
        /// <param name="model"></param>
        public CisScoring(NestedQuestions model)
        {
            QuestionsModel = model;
        }

        /// <summary>
        /// Calculates the SD Score for grouping
        /// </summary>
        /// <returns></returns>
        public Score CalculateGroupingScoreSD()
        {
            if (QuestionsModel.Groupings.FirstOrDefault()?.Questions != null)
            {
                FlattenQuestionsAndOptions(QuestionsModel.Groupings.FirstOrDefault()?.Questions);
                if (allOptionWeights.Any())
                {
                    var grouped = allOptionWeights.GroupBy(q => q.QuestionText).Select(r => new GroupedQuestions
                    {
                        QuestionText = r.Key,
                        OptionQuestions = r.ToList()
                    });
                    var sumGroupWeights = from g in grouped
                                          select new RollupOptions
                                          {
                                              Type = g.OptionQuestions.FirstOrDefault().Type,
                                              Weight = 1
                                          };
                    var sumAllWeights = (decimal)sumGroupWeights.Sum(x => x.Weight);
                    var totalGroupWeights = from g in grouped
                                            select new RollupOptions()
                                            {
                                                Type = g.OptionQuestions.FirstOrDefault().Type,
                                                Weight = 1
                                            };
                    var sumTotalWeights = (decimal)totalGroupWeights.Sum(x => x?.Weight);
                    decimal total = sumAllWeights != 0 ? sumTotalWeights / sumAllWeights : 0;
                }
            }

            return null;
        }

        /// <summary>
        /// Calculates the CIS score for a grouping/section.
        /// </summary>
        /// <returns></returns>
        public Score CalculateGroupingScore()
        {
            // Do nothing if there are no questions for some reason
            if (QuestionsModel.Groupings.FirstOrDefault()?.Questions == null)
            {
                return new Score();
            }


            FlattenQuestionsAndOptions(QuestionsModel.Groupings.FirstOrDefault()?.Questions);


            // Do nothing if there are no weights
            if (!allOptionWeights.Any())
            {
                return new Score();
            }


            var grouped = allOptionWeights.GroupBy(q => q.QuestionText).Select(r => new GroupedQuestions
            {
                QuestionText = r.Key,
                OptionQuestions = r.ToList()
            });

            var sumGroupWeights = from g in grouped
                                  select new RollupOptions
                                  {
                                      Type = g.OptionQuestions.FirstOrDefault().Type,
                                      Weight = g.OptionQuestions.FirstOrDefault().Type != "radio"
                                          ? g.OptionQuestions.Sum(x => x.Weight) 
                                          : g.OptionQuestions.MaxBy(x => x.Weight)?.Weight
                                  };

            var sumAllWeights = (decimal)sumGroupWeights.Sum(x => x.Weight);

            var totalGroupWeights = from g in grouped
                                    select new RollupOptions()
                                    {
                                        Type = g.OptionQuestions.FirstOrDefault().Type,
                                        Weight = g.OptionQuestions.FirstOrDefault().Type != "radio"
                                            ? g.OptionQuestions.Where(s => s.Selected).Sum(x => x.Weight)
                                            : g.OptionQuestions.FirstOrDefault(s => s.Selected)?.Weight
                                    };

            var sumTotalWeights = (decimal)totalGroupWeights.Sum(x => x?.Weight);



            var sgw = Newtonsoft.Json.JsonConvert.SerializeObject(sumGroupWeights);
            var tgw = Newtonsoft.Json.JsonConvert.SerializeObject(totalGroupWeights);

            var x = 1;
            

            // Now that we have handled the Option scoring, handle questions that don't have options
            //total = AddScoreForMinHrDay(total);






            decimal total = sumAllWeights != 0 ? sumTotalWeights / sumAllWeights : 0;


          



            /// special case for HYDRO, where the scores need to be grouped by impact severity and have a double for the score
            if (QuestionsModel.ModelId == 13)
            {
                var totalGroupHighThreatWeight = from g in grouped
                                                 select new RollupOptions()
                                                 {
                                                     Type = g.OptionQuestions.FirstOrDefault().Type,
                                                     Weight = g.OptionQuestions.FirstOrDefault().Type != "radio"
                                                         ? g.OptionQuestions.Where(s => s.Selected && s.ThreatType == 3).Sum(x => x.Weight)
                                                         : g.OptionQuestions.FirstOrDefault(s => s.Selected && s.ThreatType == 3)?.Weight
                                                 };
                var totalGroupMediumThreatWeight = from g in grouped
                                                   select new RollupOptions()
                                                   {
                                                       Type = g.OptionQuestions.FirstOrDefault().Type,
                                                       Weight = g.OptionQuestions.FirstOrDefault().Type != "radio"
                                                           ? g.OptionQuestions.Where(s => s.Selected && s.ThreatType == 2).Sum(x => x.Weight)
                                                           : g.OptionQuestions.FirstOrDefault(s => s.Selected && s.ThreatType == 2)?.Weight
                                                   };
                var totalGroupLowThreatWeight = from g in grouped
                                                select new RollupOptions()
                                                {
                                                    Type = g.OptionQuestions.FirstOrDefault().Type,
                                                    Weight = g.OptionQuestions.FirstOrDefault().Type != "radio"
                                                        ? g.OptionQuestions.Where(s => s.Selected && s.ThreatType == 1).Sum(x => x.Weight)
                                                        : g.OptionQuestions.FirstOrDefault(s => s.Selected && s.ThreatType == 1)?.Weight
                                                };
                var sumTotalHighThreatWeight = (double)totalGroupHighThreatWeight.Sum(x => x?.Weight);
                var sumTotalMediumThreatWeight = (double)totalGroupMediumThreatWeight.Sum(x => x?.Weight);
                var sumTotalLowThreatWeight = (double)totalGroupLowThreatWeight.Sum(x => x?.Weight);

                return new Score
                {
                    GroupingScoreDouble = (double)Math.Round(total * 100, 2, MidpointRounding.AwayFromZero),
                    LowDouble = sumTotalLowThreatWeight,
                    MediumDouble = sumTotalMediumThreatWeight,
                    HighDouble = sumTotalHighThreatWeight
                };
            }

            return new Score
            {
                GroupingScore = (int)Math.Round(total * 100, MidpointRounding.AwayFromZero),
                Low = 0,
                Median = 0,
                High = 0
            };
        }


        /// <summary>
        /// Recurse through all of the questins and options
        /// and build some flat lists.
        /// </summary>
        /// <param name="questions"></param>
        private void FlattenQuestionsAndOptions(List<Model.Nested.Question> questions)
        {
            foreach (var q in questions)
            {
                // gather the min-hr-day questions
                if (q.QuestionType != null && q.QuestionType.ToLower() == "min-hr-day")
                {
                    allMinHrDayQuestions.Add(q);
                }


                // gather all options
                allOptionWeights.AddRange(q.Options.Select(x => new FlatOption
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    OptionId = x.OptionId,
                    OptionText = x.OptionText,
                    Weight = x.Weight,
                    Selected = x.Selected,
                    Type = x.OptionType,
                    ThreatType = x.ThreatType

                }).ToList());

                foreach (var o in q.Options)
                {
                    if (o.Followups.Any())
                    {
                        FlattenQuestionsAndOptions(o.Followups);
                    }
                }

                if (q.Followups.Any())
                {
                    FlattenQuestionsAndOptions(q.Followups);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        private decimal AddScoreForMinHrDay(decimal total)
        {
            foreach (var q in allMinHrDayQuestions)
            {
                var hours = GetHours(q.AnswerMemo);

                // now we have the number of hours.  If it's less than the threshold,
                var threshold = thresholds.ContainsKey(q.QuestionId) ? thresholds[q.QuestionId] : 0;
                
            }

            return total;
        }

        
        /// <summary>
        /// Until we find an elegant place to store these thresholds in the database
        /// they live here.  There are only 13 questions that have this type of scoring.
        /// </summary>
        private Dictionary<int, double> thresholds = new Dictionary<int, double> {
            { 6094, 5.85 }
        };


        /// <summary>
        /// Parses a string like 8|day where the unit is min, hr or day
        /// and converts the numeric part to hours.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private double? GetHours(string s)
        {
            if (s == null)
            {
                return null;
            }

            if (!s.Contains('|'))
            {
                return null;
            }

            double hours = 0;

            var xxx = s.Split('|');
            var number = double.Parse(xxx[0]);
            var unit = xxx[1].ToLower();


            if (unit == "min")
            {
                hours = number / 60.0;
            }

            if (unit == "day")
            {
                hours = number * 24.0;
            }

            return hours;
        }
    }
}
