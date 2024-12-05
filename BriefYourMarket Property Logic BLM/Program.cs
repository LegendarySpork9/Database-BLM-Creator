using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Models;
using BriefYourMarketPropertyLogicBLM.Services;
using System.Configuration;

namespace BriefYourMarketPropertyLogicBLM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            LoggerService _logger = new("System");
            ApplicationService _applicationService = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, "BriefYourMarket Property Logic App Starting");

            try
            {
                _logger.LogMessage(StandardValues.LoggerValues.Info, "Loading App Settings");

                AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                AppSettingsModel.Instances = ConfigurationManager.AppSettings["Instances"];
                AppSettingsModel.FTPURL = ConfigurationManager.AppSettings["FTPURL"];
                AppSettingsModel.FTPUsername = ConfigurationManager.AppSettings["FTPUsername"];
                AppSettingsModel.FTPPassword = ConfigurationManager.AppSettings["FTPPassword"];

                _logger.LogMessage(StandardValues.LoggerValues.Info, "Loaded App Settings");
            }

            catch (Exception ex)
            {
                _logger.LogMessage(StandardValues.LoggerValues.Warning, "App Settings Loading Failed");
                _logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                Environment.Exit(0);
            }

            _applicationService.RunApplication(args);

            _logger.LogMessage(StandardValues.LoggerValues.Info, "BriefYourMarket Property Logic App Stopped");
        }
    }
}
