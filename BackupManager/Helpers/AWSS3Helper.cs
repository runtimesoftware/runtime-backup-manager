using Amazon.S3;
using Amazon.S3.Transfer;
using BackupManager.Classes;
using BackupManager.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace BackupManager.Helpers
{
    public class AWSS3Helper
    {

        /// <summary>
        /// Default Constructor for saving or retrieving configuration
        /// </summary>
        public AWSS3Helper()
        {

        }

        /// <summary>
        /// Constructor for calling UploadToS3Async Method
        /// </summary>
        /// <param name="_accessKeyId"></param>
        /// <param name="_secretKey"></param>
        /// <param name="_awsRegion"></param>
        /// <param name="_bucketName"></param>
        public AWSS3Helper(string _accessKeyId, string _secretKey, string _awsRegion, string _bucketName, string _folderName)
        {
            AccessKeyId = _accessKeyId;
            SecretKey = _secretKey;
            AwsRegion = _awsRegion;
            BucketName = _bucketName;
            FolderName = _folderName;
        }

        /// <summary>
        /// Returns config if saved
        /// </summary>
        /// <returns></returns>
        public AWSS3Setting GetConfig()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "awss3.config");
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                AWSS3Setting setting = JsonConvert.DeserializeObject<AWSS3Setting>(fileText);
                return setting;

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("error", "Unable to load AWS S3 Settings | " + Functions.GetErrorFromException(ex));
                return null;
            }
        }

        /// <summary>
        /// Saves Configuration on File
        /// </summary>
        public bool SaveConfig(AWSS3Setting setting)
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "awss3.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(setting));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }


        public string Message = "";

        readonly string AccessKeyId;
        readonly string SecretKey;
        readonly string AwsRegion;
        readonly string BucketName;
        readonly string FolderName;

        public async Task<bool> UploadToS3Async(string sourceFile, bool compressBeforeUpload = false, bool deleteAfterUpload = false)
        {

            try
            {

                AmazonS3Client client = new AmazonS3Client(AccessKeyId, SecretKey, RegionFromName(AwsRegion));
                TransferUtility utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                DateTime thistime = DateTime.UtcNow;

                if (!File.Exists(sourceFile))
                {
                    Message = "File not found. " + sourceFile;
                    return false;
                }

                string fileToUpload = sourceFile;
                if (compressBeforeUpload)
                {
                    //save zip at temporary location
                    string zipPath = Path.Combine(EnVar.AppTempPath, Path.GetFileName(sourceFile) + ".zip");
                    FileStream zipStream = new FileStream(zipPath, FileMode.Create);
                    using (ZipArchive zipFile = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        var zipEntry = zipFile.CreateEntry(sourceFile);
                    }
                    fileToUpload = zipPath;
                }

                string BucketPath = BucketName;
                if (!string.IsNullOrWhiteSpace(FolderName))
                {
                    string folderName = FolderName;
                    folderName = folderName.Replace("%Y%", DateTime.UtcNow.Year.ToString());
                    folderName = folderName.Replace("%M%", DateTime.UtcNow.Month.ToString());
                    folderName = folderName.Replace("%D%", DateTime.UtcNow.Day.ToString());
                    BucketPath += "/" + folderName;
                }

                request.BucketName = BucketPath; 
                request.Key = Path.GetFileName(fileToUpload);
                request.FilePath = fileToUpload;
                await utility.UploadAsync(request);

                //delete zip archive (if compression active)
                if (compressBeforeUpload) File.Delete(fileToUpload);

                //delete files after upload (if configured)
                if (deleteAfterUpload == true) File.Delete(sourceFile);

                Message = "File successfully uploaded";
                return true;

            }
            catch (Exception ex)
            {
                Message = Functions.GetErrorFromException(ex);
                //Logger.LogIt("error", "/api/appDaily | Database backup to AWS S3 failed | " + ex.Message, "system");
                return false;
            }

        }

        /// <summary>
        /// Converts Region string to RegionEndPoint.
        /// Although AWSSDK has a function Amazon.RegionEndpoint.GetBySystemName(awsRegion) to convert string name into region endpoint but that is not working correctly.
        /// </summary>
        /// <param name="AwsRegion"></param>
        /// <returns></returns>
        public Amazon.RegionEndpoint RegionFromName(string AwsRegion)
        {
            switch (AwsRegion)
            {
                case "AFSouth1":
                    return Amazon.RegionEndpoint.AFSouth1;
                case "APEast1":
                    return Amazon.RegionEndpoint.APEast1;
                case "APNortheast1":
                    return Amazon.RegionEndpoint.APNortheast1;
                case "APNortheast2":
                    return Amazon.RegionEndpoint.APNortheast2;
                case "APNortheast3":
                    return Amazon.RegionEndpoint.APNortheast3;
                case "APSouth1":
                    return Amazon.RegionEndpoint.APSouth1;
                case "APSoutheast1":
                    return Amazon.RegionEndpoint.APSoutheast1;
                case "APSoutheast2":
                    return Amazon.RegionEndpoint.APSoutheast2;
                case "CACentral1":
                    return Amazon.RegionEndpoint.CACentral1;
                case "CNNorth1":
                    return Amazon.RegionEndpoint.CNNorth1;
                case "CNNorthWest1":
                    return Amazon.RegionEndpoint.CNNorthWest1;
                case "EUCentral1":
                    return Amazon.RegionEndpoint.EUCentral1;
                case "EUNorth1":
                    return Amazon.RegionEndpoint.EUNorth1;
                case "EUSouth1":
                    return Amazon.RegionEndpoint.EUSouth1;
                case "EUWest1":
                    return Amazon.RegionEndpoint.EUWest1;
                case "EUWest2":
                    return Amazon.RegionEndpoint.EUWest2;
                case "EUWest3":
                    return Amazon.RegionEndpoint.EUWest2;
                case "MESouth1":
                    return Amazon.RegionEndpoint.MESouth1;
                case "SAEast1":
                    return Amazon.RegionEndpoint.SAEast1;
                case "USEast1":
                    return Amazon.RegionEndpoint.USEast1;
                case "USEast2":
                    return Amazon.RegionEndpoint.USEast2;
                case "USGovCloudEast1":
                    return Amazon.RegionEndpoint.USGovCloudEast1;
                case "USGovCloudWest1":
                    return Amazon.RegionEndpoint.USGovCloudWest1;
                case "USWest1":
                    return Amazon.RegionEndpoint.USWest1;
                case "USWest2":
                    return Amazon.RegionEndpoint.USWest2;
                default:
                    return Amazon.RegionEndpoint.USWest1;
            }
        }

    }
}
