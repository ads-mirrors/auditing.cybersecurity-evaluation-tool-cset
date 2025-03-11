//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;

namespace CSETWebCore.Model.Aggregation
{
    public class MergeCategory
    {
        public string Category { get; set; }
        public List<MergeQuestion> Questions = new List<MergeQuestion>();
    }
}