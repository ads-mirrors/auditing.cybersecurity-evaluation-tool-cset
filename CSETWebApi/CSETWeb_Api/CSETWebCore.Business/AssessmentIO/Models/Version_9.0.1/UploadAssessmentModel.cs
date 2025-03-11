//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;

namespace CSETWebCore.Business.ImportAssessment.Models.Version_9_0_1
{
    /// <summary>
    /// this is the general framework for what I need 
    /// to move an assessment.
    /// </summary>
    public class UploadAssessmentModel
    {
        public List<jCSET_VERSION> jCSET_VERSION { get; set; }
        public List<jASSESSMENT_SELECTED_LEVELS> jASSESSMENT_SELECTED_LEVELS { get; set; }
        public List<jAVAILABLE_STANDARDS> jAVAILABLE_STANDARDS { get; set; }
        public List<jCNSS_CIA_JUSTIFICATIONS> jCNSS_CIA_JUSTIFICATIONS { get; set; }
        public List<jASSESSMENTS> jASSESSMENTS { get; set; }
        public List<jCUSTOM_QUESTIONAIRES> jCUSTOM_QUESTIONAIRES { get; set; }
        public List<jCUSTOM_BASE_STANDARDS> jCUSTOM_BASE_STANDARDS { get; set; }
        public List<jSTANDARD_SELECTION> jSTANDARD_SELECTION { get; set; }
        public List<jCUSTOM_STANDARD_BASE_STANDARD> jCUSTOM_STANDARD_BASE_STANDARD { get; set; }
        public List<jCUSTOM_QUESTIONAIRE_QUESTIONS> jCUSTOM_QUESTIONAIRE_QUESTIONS { get; set; }
        public List<jADDRESS> jADDRESS { get; set; }
        public List<jDOCUMENT_FILE> jDOCUMENT_FILE { get; set; }
        public List<jFRAMEWORK_TIER_TYPE_ANSWER> jFRAMEWORK_TIER_TYPE_ANSWER { get; set; }
        public List<jSUB_CATEGORY_ANSWERS> jSUB_CATEGORY_ANSWERS { get; set; }
        public List<jASSESSMENT_CONTACTS> jASSESSMENT_CONTACTS { get; set; }

        public List<jFINDING> jFINDING { get; set; }
        public List<jANSWER> jANSWER { get; set; }
        public List<jNIST_SAL_INFO_TYPES> jNIST_SAL_INFO_TYPES { get; set; }
        public List<jDOCUMENT_ANSWERS> jDOCUMENT_ANSWERS { get; set; }
        public List<jGENERAL_SAL> jGENERAL_SAL { get; set; }
        public List<jNIST_SAL_QUESTION_ANSWERS> jNIST_SAL_QUESTION_ANSWERS { get; set; }
        public List<jINFORMATION> jINFORMATION { get; set; }
        public List<jDEMOGRAPHICS> jDEMOGRAPHICS { get; set; }
        public List<jPARAMETER_VALUES> jPARAMETER_VALUES { get; set; }
        public List<jPARAMETER_ASSESSMENT> jPARAMETER_ASSESSMENTs { get; set; }

        public List<jFINDING_CONTACT> jFINDING_CONTACT { get; set; }
        public List<jUSER_DETAIL_INFORMATION> jUSER_DETAIL_INFORMATION { get; set; }

        public List<jFINANCIAL_HOURS> jFINANCIAL_HOURS { get; set; }
        public List<jFINANCIAL_ASSESSMENT_VALUES> jFINANCIAL_ASSESSMENT_VALUES { get; set; }
        public List<jASSESSMENTS_REQUIRED_DOCUMENTATION> jASSESSMENTS_REQUIRED_DOCUMENTATION { get; set; }
        public List<jASSESSMENT_IRP_HEADER> jASSESSMENT_IRP_HEADER { get; set; }
        public List<jASSESSMENT_IRP> jASSESSMENT_IRP { get; set; }

        public List<string> CustomStandards { get; set; }
        public List<string> CustomStandardDocs { get; set; }

        public UploadAssessmentModel()
        {
            this.jADDRESS = new List<jADDRESS>();
            this.jANSWER = new List<jANSWER>();
            this.jASSESSMENTS = new List<jASSESSMENTS>();
            this.jASSESSMENTS_REQUIRED_DOCUMENTATION = new List<jASSESSMENTS_REQUIRED_DOCUMENTATION>();
            this.jASSESSMENT_CONTACTS = new List<jASSESSMENT_CONTACTS>();
            this.jASSESSMENT_IRP = new List<jASSESSMENT_IRP>();
            this.jASSESSMENT_IRP_HEADER = new List<jASSESSMENT_IRP_HEADER>();
            this.jASSESSMENT_SELECTED_LEVELS = new List<jASSESSMENT_SELECTED_LEVELS>();
            this.jAVAILABLE_STANDARDS = new List<jAVAILABLE_STANDARDS>();
            this.jCNSS_CIA_JUSTIFICATIONS = new List<jCNSS_CIA_JUSTIFICATIONS>();
            this.jCSET_VERSION = new List<jCSET_VERSION>();
            this.jCUSTOM_BASE_STANDARDS = new List<jCUSTOM_BASE_STANDARDS>();
            this.jCUSTOM_QUESTIONAIRES = new List<jCUSTOM_QUESTIONAIRES>();
            this.jCUSTOM_QUESTIONAIRE_QUESTIONS = new List<jCUSTOM_QUESTIONAIRE_QUESTIONS>();
            this.jCUSTOM_STANDARD_BASE_STANDARD = new List<jCUSTOM_STANDARD_BASE_STANDARD>();
            this.jDEMOGRAPHICS = new List<jDEMOGRAPHICS>();
            this.jDOCUMENT_ANSWERS = new List<jDOCUMENT_ANSWERS>();
            this.jDOCUMENT_FILE = new List<jDOCUMENT_FILE>();
            this.jFINANCIAL_HOURS = new List<jFINANCIAL_HOURS>();
            this.jFINANCIAL_ASSESSMENT_VALUES = new List<jFINANCIAL_ASSESSMENT_VALUES>();
            this.jFINDING = new List<jFINDING>();
            this.jFINDING_CONTACT = new List<jFINDING_CONTACT>();
            this.jFRAMEWORK_TIER_TYPE_ANSWER = new List<jFRAMEWORK_TIER_TYPE_ANSWER>();
            this.jGENERAL_SAL = new List<jGENERAL_SAL>();
            this.jINFORMATION = new List<jINFORMATION>();
            this.jNIST_SAL_INFO_TYPES = new List<jNIST_SAL_INFO_TYPES>();
            this.jNIST_SAL_QUESTION_ANSWERS = new List<jNIST_SAL_QUESTION_ANSWERS>();
            this.jPARAMETER_VALUES = new List<jPARAMETER_VALUES>();
            this.jPARAMETER_ASSESSMENTs = new List<jPARAMETER_ASSESSMENT>();
            this.jSTANDARD_SELECTION = new List<jSTANDARD_SELECTION>();
            this.jSUB_CATEGORY_ANSWERS = new List<jSUB_CATEGORY_ANSWERS>();
            this.jUSER_DETAIL_INFORMATION = new List<jUSER_DETAIL_INFORMATION>();
            this.CustomStandardDocs = new List<string>();
            this.CustomStandards = new List<string>();
        }
    }
}


