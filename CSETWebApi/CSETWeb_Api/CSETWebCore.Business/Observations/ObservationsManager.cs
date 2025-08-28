//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Observations;
using CSETWebCore.Model.Set;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSETWebCore.Business.Observations
{
    public class ObservationsManager
    {
        private int _assessmentId;

        /** I need to get the list of contacts
        *  provide a list of all the available contacts 
        *  allow the user to select from the list
        *  restrict this to one observation only
        */

        private CSETContext _context;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="assessmentId"></param>
        public ObservationsManager(CSETContext context, int assessmentId)
        {
            _assessmentId = assessmentId;
            _context = context;

            TinyMapper.Bind<FINDING, Observation>();
            TinyMapper.Bind<FINDING_CONTACT, ObservationContact>(config => config.Bind(source => source.Finding_Id, target => target.Observation_Id));
            TinyMapper.Bind<IMPORTANCE, Importance>();
            TinyMapper.Bind<ASSESSMENT_CONTACTS, ObservationContact>();
            TinyMapper.Bind<Observation, ObservationContact>();
            TinyMapper.Bind<Observation, Observation>();
            TinyMapper.Bind<ObservationContact, ObservationContact>();
        }


        /// <summary>
        /// Returns a list of Observations (FINDING databse records) for an answer
        /// </summary>
        public List<Observation> AllObservations(int answerId)
        {
            List<Observation> observations = new List<Observation>();

            var observationsForAnswer = _context.FINDING
                .Where(x => x.Answer_Id == answerId)
                .Include(i => i.Importance)
                .Include(k => k.FINDING_CONTACT)
                .ToList();

            foreach (FINDING o in observationsForAnswer)
            {
                Observation obs = new Observation();
                obs.Observation_Contacts = new List<ObservationContact>();
                obs.Summary = o.Summary;
                obs.Observation_Id = o.Finding_Id;
                obs.Answer_Id = answerId;
                TinyMapper.Map(o, obs);
                if (o.Importance == null)
                    obs.Importance = new Importance()
                    {
                        Importance_Id = 1,
                        Value = Constants.Constants.SAL_LOW
                    };
                else
                    obs.Importance = TinyMapper.Map<IMPORTANCE, Importance>(o.Importance);

                foreach (FINDING_CONTACT fc in o.FINDING_CONTACT)
                {
                    ObservationContact webFc = TinyMapper.Map<FINDING_CONTACT, ObservationContact>(fc);

                    webFc.Observation_Id = fc.Finding_Id;
                    webFc.Selected = (fc != null);
                    obs.Observation_Contacts.Add(webFc);
                }
                observations.Add(obs);
            }
            return observations;
        }


        /// <summary>
        /// 
        /// </summary>
        public Observation GetObservation(int observationId, int answerId = 0)
        {
            // look for an existing FINDING record.  If not, create one.
            FINDING f = _context.FINDING
                    .Where(x => x.Finding_Id == observationId)
                    .Include(fc => fc.FINDING_CONTACT)
                    .FirstOrDefault();

            if (f == null)
            {
                f = new FINDING()
                {
                    Answer_Id = answerId
                };

                _context.FINDING.Add(f);
                _context.SaveChanges();
            }


            // Create an Observation object from the FINDING and joined tables
            Observation obs;
            var q = _context.ANSWER.Where(x => x.Answer_Id == f.Answer_Id).FirstOrDefault();

            obs = TinyMapper.Map<Observation>(f);
            obs.Observation_Id = f.Finding_Id;
            obs.Question_Id = q != null ? q.Question_Or_Requirement_Id : 0;

            obs.Observation_Contacts = new List<ObservationContact>();
            foreach (var contact in _context.ASSESSMENT_CONTACTS.Where(x => x.Assessment_Id == _assessmentId))
            {
                ObservationContact webContact = TinyMapper.Map<ObservationContact>(contact);
                webContact.Name = contact.PrimaryEmail + " -- " + contact.FirstName + " " + contact.LastName;
                webContact.Selected = (f.FINDING_CONTACT.Where(x => x.Assessment_Contact_Id == contact.Assessment_Contact_Id).FirstOrDefault() != null);
                obs.Observation_Contacts.Add(webContact);
            }

            return obs;
        }


        /// <summary>
        /// Deletes an Observation (FINDING record)
        /// </summary>
        /// <param name="observation"></param>
        public void DeleteObservation(Observation observation)
        {
            ObservationData fm = new ObservationData(observation, _context);
            fm.Delete();
            fm.Save();
        }


        /// <summary>
        /// Updates an Observation in its FINDING database record
        /// </summary>
        /// <param name="observation"></param>
        public int UpdateObservation(Observation observation)
        {
            ObservationData fm = new ObservationData(observation, _context);
            int id = fm.Save();
            return id;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<ActionItems> GetActionItems(int parentId, int observation_id)
        {
            var actionItems = new List<ActionItems>();

            var table = from questions in _context.MATURITY_QUESTIONS
                        join actions in _context.ISE_ACTIONS on questions.Mat_Question_Id equals actions.Mat_Question_Id
                        join o in _context.ISE_ACTIONS_FINDINGS on new { Mat_Question_Id = questions.Mat_Question_Id, Finding_Id = observation_id }
                            equals new { Mat_Question_Id = o.Mat_Question_Id, Finding_Id = o.Finding_Id }
                           into overrides
                        from o in overrides.DefaultIfEmpty()
                        orderby questions.Mat_Question_Id ascending
                        where questions.Parent_Question_Id == parentId
                        select new { actions = actions, overrides = o };
            foreach (var row in table.ToList())
            {
                actionItems.Add(
                    new ActionItems()
                    {
                        Question_Id = row.actions.Mat_Question_Id,
                        Description = row.actions.Description,
                        Action_Items = row.overrides == null
                        ? row.actions.Action_Items : row.overrides.Action_Items_Override,
                        Regulatory_Citation = row.actions.Regulatory_Citation
                    }
                );
            }
            return actionItems;
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateIssues(ActionItemTextUpdate items)
        {
            foreach (var item in items.actionTextItems)
            {
                var save = _context.ISE_ACTIONS_FINDINGS.Where(x => x.Finding_Id == items.observation_Id && x.Mat_Question_Id == item.Mat_Question_Id).FirstOrDefault();
                if (save == null)
                {
                    _context.ISE_ACTIONS_FINDINGS.Add(new ISE_ACTIONS_FINDINGS()
                    {
                        Mat_Question_Id = item.Mat_Question_Id,
                        Finding_Id = items.observation_Id,
                        Action_Items_Override = item.ActionItemOverrideText
                    });
                    _context.SaveChanges();
                }
                else
                {
                    save.Action_Items_Override = item.ActionItemOverrideText;
                    _context.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Creates an Observation based on maturity question properties.
        /// </summary>
        public void BuildAutoObservation(Model.Question.Answer answer)
        {
            // get question ID
            var questionId = answer.QuestionId;

            // get OBS-properties for the question
            var props = _context.MATURITY_QUESTION_PROPS.Where(x => x.Mat_Question_Id == questionId && x.PropertyName.StartsWith("OBS-")).ToList();


            // see if we have an existing auto observation
            var existingAutoObs = _context.FINDING.Where(x => x.Answer_Id == answer.AnswerId && x.Auto_Generated == Constants.Constants.ObsCreatedByVadr).FirstOrDefault();
            if (existingAutoObs != null)
            {
                return;
            }


            // create new observation record
            var newObs = GetObservation(-1, (int)answer.AnswerId);

            // populate with properties
            newObs.Summary = props.Where(x => x.PropertyName == "OBS-DISCOVERY").FirstOrDefault()?.PropertyValue ?? "";
            newObs.Issue = props.Where(x => x.PropertyName == "OBS-RISK-STATEMENT").FirstOrDefault()?.PropertyValue ?? "";
            newObs.Recommendations = props.Where(x => x.PropertyName == "OBS-RECOMMENDATION").FirstOrDefault()?.PropertyValue ?? "";

            //newObs.Risk_Area = props.Where(x => x.PropertyName == "OBS-RISK").FirstOrDefault()?.PropertyValue ?? "";
            //newObs.Impact = props.Where(x => x.PropertyName == "OBS-IMPACT").FirstOrDefault()?.PropertyValue ?? "";
            //newObs.Vulnerabilities = props.Where(x => x.PropertyName == "OBS-VULNERABILITY").FirstOrDefault()?.PropertyValue ?? "";

            // mark the observation as created by VADR so that we can find it easily to delete 
            newObs.Auto_Generated = Constants.Constants.ObsCreatedByVadr;

            UpdateObservation(newObs);
        }


        /// <summary>
        /// Deletes all "Auto" Observations attached to the specified Answer.
        /// </summary>
        /// <param name="answer"></param>
        public void DeleteAutoObservation(Model.Question.Answer answer)
        {
            var listObs = _context.FINDING.Where(x => x.Answer_Id == answer.AnswerId && x.Auto_Generated == Constants.Constants.ObsCreatedByVadr).ToList();
            _context.RemoveRange(listObs);
            _context.SaveChanges();
        }
    }
}