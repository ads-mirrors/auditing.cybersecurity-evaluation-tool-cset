﻿//////////////////////////////// 
// 
//   Copyright 2024 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.Business.Authorization;
using CSETWebCore.Business.Contact;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Helpers;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.Authentication;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CSETWebCore.Api.Controllers
{
    /// <summary>
    /// Manages conversion of assessment type(s) for an assessment.  
    /// </summary>
    [ApiController]
    public class ConversionController : ControllerBase
    {
        private readonly CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;
        private readonly ITokenManager _tokenManager;
        static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ConversionController(CSETContext context, ITokenManager tokenManager, IAssessmentUtil assessmentUtil)
        {
            _context = context;
            _tokenManager = tokenManager;
            _assessmentUtil = assessmentUtil;
        }


        /// <summary>
        /// Indicates if the assessment is a Cyber Florida "entry" assessment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/convert/cf/entry")]
        public IActionResult IsEntryCF()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new ConversionBusiness(_context, _assessmentUtil);
            return Ok(biz.IsEntryCF(assessmentId));
        }
        [HttpPost]
        [Route("api/convert/cf/entrys")]
        public IActionResult IsEntrysCF(List<int> assessmentIds)
        {   
            var biz = new ConversionBusiness(_context, _assessmentUtil);
            return Ok(biz.IsEntryCF(assessmentIds));
        }


        /// <summary>
        /// Indicates if the assessment is a Cyber Florida "mid" assessment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/convert/cf/mid")]
        public IActionResult IsMidCF()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new ConversionBusiness(_context, _assessmentUtil);
            return Ok(biz.IsMidCF(assessmentId));
        }


        /// <summary>
        /// Converts a Cyber Florida "entry" assessment to a full assessment.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/convert/cf")]
        public IActionResult ConvertCF()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            var biz = new ConversionBusiness(_context, _assessmentUtil);
            biz.ConvertCF(assessmentId);

            return Ok();
        }


        /// <summary>
        /// Converts a Cyber Florida "entry" assessment to a "mid-level" assessment.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/convert/entryToMid")]
        public IActionResult ConvertEntryToMid()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            var biz = new ConversionBusiness(_context, _assessmentUtil);
            biz.ConvertEntryToMid(assessmentId);

            return Ok();
        }


        /// <summary>
        /// Converts a Cyber Florida "mid" assessment to a full assessment.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/convert/midToFull")]
        public IActionResult ConvertMidToFull()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            var biz = new ConversionBusiness(_context, _assessmentUtil);
            biz.ConvertMidToFull(assessmentId);

            return Ok();
        }

        [HttpGet]
        [Route("api/cf/isComplete")]
        public IActionResult IsCFComplete()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new CFBusiness(_context, _assessmentUtil);
            return Ok(biz.getInitialAnswers(assessmentId));
        }

        [HttpGet]
        [Route("api/cf/isMidComplete")]
        public IActionResult IsCFMidComplete()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new CFBusiness(_context, _assessmentUtil);
            return Ok(biz.getMidInitialAnswers(assessmentId));
        }

        [HttpGet]
        [Route("api/cf/getAnswerBreakdownForBarChart")]
        public IActionResult GetAnswerBreakdownForBarChart()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new CFBusiness(_context, _assessmentUtil);
            return Ok(biz.getBarChartInfo(assessmentId));
        }

        [HttpGet]
        [Route("api/cf/getScoreBreakdown")]
        public async Task<IActionResult> GetScoreBreakdown()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new CFBusiness(_context, _assessmentUtil);
            try
            {
                var temp = await biz.getGroupingScores(assessmentId);
                return Ok(temp);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Ok(biz.getGroupingScores(assessmentId));

        }

        [HttpGet]
        [Route("api/cf/getTop5Lowest")]
        public async Task<IActionResult> GetTop5Lowest()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var biz = new CFBusiness(_context, _assessmentUtil);
            // 5-lowest parsing is done on frontend
            return Ok(await biz.Top5LowestScoredForAllSubcats(assessmentId));

        }
    }
}
