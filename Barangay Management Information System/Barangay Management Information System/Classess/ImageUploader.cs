using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Barangay_Management_Information_System.Classess
{
    public class ImageUploader
    {

        private DBEntities ent = new DBEntities();

        public void SaveImage(HttpPostedFileBase image, string accountId)
        {
            
            string defaultPath = "/Content/Images/User Profile Photo/";

            AccountAsset oldAsset = ent.AccountAssets.Where(m => m.AccountId == accountId).FirstOrDefault();

            if (oldAsset != null)
            {

                LocationStorage oldLocationStorage = oldAsset.LocationStorage;

                string oldImageAddress = oldLocationStorage.Address;

                File.Delete(HostingEnvironment.MapPath(oldImageAddress));

                ent.AccountAssets.Remove(oldAsset);
                ent.SaveChanges();

                ent.LocationStorages.Remove(oldLocationStorage);
                ent.SaveChanges();

            }    
            
            string fileName, fileExtension = "";

            fileName = Path.GetFileNameWithoutExtension(image.FileName);
            fileExtension = Path.GetExtension(image.FileName);          

            fileName += accountId;

            if (IsExisting(defaultPath, fileName, fileExtension))
            {
                fileName += new Random().Next(101);
            }

            // variable imgPath would be stored, contains the actual location of the image.
            string imgPath = defaultPath + fileName;

            //fileName = Path.Combine(Server.MapPath((defaultPath)), fileName);
            string filePath = HostingEnvironment.MapPath(defaultPath + fileName + fileExtension);

            image.SaveAs(filePath);

            // inserts first to the storage to get the fileId
            LocationStorage storage = new LocationStorage()
            {
                LocationStorageId = KeyGenerator.GenerateId(fileName + accountId),
                Name = fileName,
                Address = defaultPath + fileName + fileExtension
            };
            ent.LocationStorages.Add(storage);
            ent.SaveChanges();

            AccountAsset asset = new AccountAsset()
            {
                AccountAssetId = KeyGenerator.GenerateId(fileName),
                AccountId = accountId,
                LocationStorageId = storage.LocationStorageId
            };
            ent.AccountAssets.Add(asset);
            ent.SaveChanges();                        

        }

        public bool IsExisting(string defPath, string fileName, string fileExtension)
        {
            var relativePath = defPath + fileName + fileExtension;
            string filePath = HostingEnvironment.MapPath(defPath + fileName + fileExtension);

            if (System.IO.File.Exists(filePath))
            {
                return true;
            }

            return false;
        }

    }
}