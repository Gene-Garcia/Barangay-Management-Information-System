using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Classess
{
    public class UserHelper
    {
        public static string GetDisplayPicture(string userId, DBEntities entities)
        {
            try
            {
                string locationStorageId = entities.AccountAssets.Where(m => m.AccountId == userId).Select(m => m.LocationStorageId).FirstOrDefault();
                return entities.LocationStorages.Where(m => m.LocationStorageId == locationStorageId).Select(m => m.Address).FirstOrDefault();
            }
            catch (Exception e)
            {
                return "";
            }            
        }
    }
}