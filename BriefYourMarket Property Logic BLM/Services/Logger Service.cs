using log4net;

namespace BriefYourMarketPropertyLogicBLM.Services
{
    internal class LoggerService
    {
        private readonly ILog Logger = LogManager.GetLogger("Logs");
        private readonly string Identity;

        public LoggerService(string report)
        {
            Identity = report;
        }

        public void LogMessage(string level, string message)
        {
            switch (level)
            {
                case "Info": Console.ForegroundColor = ConsoleColor.Green; Logger.Info($"{Identity} - {message}"); Console.ForegroundColor = ConsoleColor.Gray; break;
                case "Debug": Console.ForegroundColor = ConsoleColor.Yellow; Logger.Debug($"{Identity} - {message}"); Console.ForegroundColor = ConsoleColor.Gray; break;
                case "Warn": Console.ForegroundColor = ConsoleColor.Red; Logger.Warn($"{Identity} - {message}"); Console.ForegroundColor = ConsoleColor.Gray; break;
                case "Error": Logger.Error($"{Identity} - {message}"); break;
            }
        }
    }
}
