﻿//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 

namespace CSETWebCore.Business.AssessmentIO.Import
{
    internal class CSET_09_2_1_to_09_2_2_Upgrade : ICSETJSONFileUpgrade
    {
        /// <summary>
        /// this is the string we will be upgrading to
        /// </summary>
        static string versionString = "9.2.2";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ExecuteUpgrade(string json)
        {
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Version GetVersion()
        {
            return ImportUpgradeManager.ParseVersion(versionString);
        }
    }
}
