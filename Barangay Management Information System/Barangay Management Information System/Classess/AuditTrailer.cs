using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Classess
{
    public class AuditTrailer
    {
        private static DBEntities entities = new DBEntities();

        // Constant Audit Types
        public const int ACCOUNT_TYPE               = 568556;
        public const int LOGIN_TYPE                 = 451377;
        public const int BARANGAY_CLEARANCE_TYPE    = 112346;
        public const int BARANGAY_OFFICIAL_TYPE     = 753721;
        public const int RESIDENT_TYPE              = 342523;
        public const int SUMMON_TYPE                = 188853;

        private const string INVALID_ACTION         = "x09jF";

        public void Record(string message, int actionType, string userId)
        {

            try
            {

                string auditActionId = DetermineActionId(actionType);

                if (auditActionId != INVALID_ACTION)
                {

                    string id = CreateProperId(message, actionType);

                    AuditTrail auditTrail = new AuditTrail()
                    {
                        AuditTrailId = id,
                        AccountId = userId,
                        AuditActionsId = auditActionId,
                        Message = message,
                        DateAction = DateTime.Now
                    };

                    entities.AuditTrails.Add(auditTrail);
                    entities.SaveChanges();
                }

            }
            catch (Exception e) { }

        }

        private string CreateProperId(string message, int actionType)
        {
            string id = "";
            AuditTrail auditTrail;
            do
            {
                id = KeyGenerator.GenerateId(message + actionType + "for-audit");

                auditTrail = entities.AuditTrails.Where(m => m.AuditTrailId == id).FirstOrDefault();

                if (auditTrail == null)
                    break;

            } while (true);

            return id;
        }

        private string DetermineActionId(int actionType)
        {
            string id = "";

            if (actionType == ACCOUNT_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "account").Select(m => m.AuditActionsId).FirstOrDefault();
            else if (actionType == LOGIN_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "login").Select(m => m.AuditActionsId).FirstOrDefault();
            else if (actionType == BARANGAY_CLEARANCE_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "barangay clearance").Select(m => m.AuditActionsId).FirstOrDefault();
            else if (actionType == BARANGAY_OFFICIAL_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "barangay official").Select(m => m.AuditActionsId).FirstOrDefault();
            else if (actionType == RESIDENT_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "resident").Select(m => m.AuditActionsId).FirstOrDefault();
            else if (actionType == SUMMON_TYPE)
                id = entities.AuditActions.Where(m => m.Name.ToLower() == "summon").Select(m => m.AuditActionsId).FirstOrDefault();
            else
                id = INVALID_ACTION;

            return id;
        }

    }
}