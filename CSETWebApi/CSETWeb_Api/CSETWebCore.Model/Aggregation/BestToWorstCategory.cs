//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;

namespace CSETWebCore.Model.Aggregation
{
    public class BestToWorstCategory
    {
        public string Category { get; set; }
        public List<GetComparisonBestToWorst> Assessments { get; set; }
    }
}