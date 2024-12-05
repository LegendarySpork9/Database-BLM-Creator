using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Models;
using System.Net;
using System.Reflection;

namespace BriefYourMarketPropertyLogicBLM.Services
{
    internal class FTPService
    {
        public void TestConnection()
        {
            LoggerService _logger = new("System");

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Testing FTP connection wtih test file");

            try
            {
                WebClient client = new();

                string address = AppSettingsModel.FTPURL + "test1267784.ZIP";

                client.Credentials = new NetworkCredential(AppSettingsModel.FTPUsername, AppSettingsModel.FTPPassword);
                client.UploadFile(address, WebRequestMethods.Ftp.UploadFile, ".\\Test File\\test1267784.ZIP");
                client.Dispose();

                _logger.LogMessage(StandardValues.LoggerValues.Debug, "Successfully uploaded test file");
            }

            catch (Exception ex)
            {
                _logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to test FTP connection");
                _logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Tested FTP connection with test file");
        }

        public void UploadZip(LoggerService _logger, string path, string fileName)
        {
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploading {fileName} to Property Logic");

            try
            {
                WebClient client = new();

                string address = AppSettingsModel.FTPURL + fileName;

                client.Credentials = new NetworkCredential(AppSettingsModel.FTPUsername, AppSettingsModel.FTPPassword);
                client.UploadFile(address, WebRequestMethods.Ftp.UploadFile, path);
                client.Dispose();

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploaded {fileName} to Property Logic");
            }

            catch (Exception ex)
            {
                _logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to upload {fileName} to Property Logic");
                _logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }
        }
    }
}
