//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;

namespace CSETWebCore.Business.Reports
{
    public class AggregationReportData
    {
        public string AggregationName { get; set; }
        public List<BasicReportData.OverallSALTable> SalList { get; set; }
        public List<DocumentLibraryEntry> DocumentLibraryEntries { get; set; }
        public AggInformation Information { get; set; }
    }
}