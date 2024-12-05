using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Functions;
using BriefYourMarketPropertyLogicBLM.Models;
using System.IO.Compression;

namespace BriefYourMarketPropertyLogicBLM.Services
{
    internal class DocumentService
    {
        private LoggerService Logger;

        public DocumentService(LoggerService _logger)
        {
            Logger = _logger;
        }

        public (string, string, bool) CreateBLM(string instance, BranchModel branch)
        {
            DocumentFunction _documentFunction = new();

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating BLM for {branch.Id}");

            string blmDirectory = $".\\Data\\{instance}\\{branch.Id}";
            string blmName = $"{branch.Id.Replace(" ", "")}_{DateTime.UtcNow:yyyyMMddHHmm}.blm";
            string blmPath = $@"{blmDirectory}\{blmName}";
            string blmTemplate = File.ReadAllText(".\\Template\\Template.blm");
            (char endOfField, char endofRow) = GetEndOf();
            string[] fields = GetBLMFields();
            string data = "";
            int properties = branch.Properties.Count;
            int propertyCount = 0;
            bool success = false;

            _documentFunction.CreateDirectory(blmDirectory);

            try
            {
                File.Create(blmPath).Close();
                File.AppendAllText(blmPath, blmTemplate);

                for (int i = 0; i < properties; i++)
                {
                    data += _documentFunction.FormatPropertyRecord(branch.Properties[i], endOfField, endofRow, fields, branch.Id.Replace(" ", ""));
                    data += "\n";
                    propertyCount++;

                    if (propertyCount == 25)
                    {
                        data = data.Remove(data.LastIndexOf("\n"));

                        File.AppendAllText(blmPath, data);

                        data = "";
                        propertyCount = 0;

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, "Added 25 properties to document");
                    }
                }

                if (!string.IsNullOrWhiteSpace(data))
                {
                    data += "#END#";

                    File.AppendAllText(blmPath, data);

                    Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Added {propertyCount} properties to document");
                }

                success = true;
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Created BLM for {branch.Id}");
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to create BLM for {branch.Id}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (blmPath, blmName, success);
        }

        private (char, char) GetEndOf()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining BLM End of characters");

            char endOfField = new();
            char endOfRow = new();

            try
            {
                string[] lines = File.ReadAllLines(".\\Template\\Template.BLM");

                foreach (string line in lines)
                {
                    if (line.Contains("EOF"))
                    {
                        int index = line.IndexOf("'");
                        endOfField = char.Parse(line.Remove(0, index).Replace("'", ""));

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"End of Field: {endOfField}");
                    }

                    if (line.Contains("EOR"))
                    {
                        int index = line.IndexOf("'");
                        endOfRow = char.Parse(line.Remove(0, index).Replace("'", ""));

                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"End of Row: {endOfRow}");
                    }
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained BLM End of Characters");
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Obtaining BLM End of Characters Failed");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (endOfField, endOfRow);
        }

        private string[] GetBLMFields()
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining BLM Fields");

            string[] fields = Array.Empty<string>();

            try
            {
                string[] lines = File.ReadAllLines(".\\Template\\Template.BLM");
                List<string> templateLines = new();
                string endOfField = "";
                string endOfRow = "";

                foreach (string line in lines)
                {
                    if (line.Contains("EOF"))
                    {
                        int index = line.IndexOf("'");
                        endOfField = line.Remove(0, index).Replace("'", "");
                    }

                    if (line.Contains("EOR"))
                    {
                        int index = line.IndexOf("'");
                        endOfRow = line.Remove(0, index).Replace("'", "");
                    }

                    templateLines.Add(line);
                }

                int definitionIndex = templateLines.IndexOf("#DEFINITION#");

                for (int i = definitionIndex; i != templateLines.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(templateLines[i]) && templateLines[i].Contains(endOfField))
                    {
                        Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Fields: {templateLines[i]}");
                        fields = templateLines[i].Replace(endOfRow, "").Split(endOfField);
                        break;
                    }
                }

                Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained BLM Fields");
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Obtaining BLM Fields Failed");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return fields;
        }

        public void CreateZIP(string blmPath)
        {
            try
            {
                ZipArchive zip = ZipFile.Open(blmPath.Replace(".blm", ".zip"), ZipArchiveMode.Create);

                zip.CreateEntryFromFile(blmPath, Path.GetFileName(blmPath), CompressionLevel.Optimal);
                zip.Dispose();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Failed to ZIP BLM file");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }
        }
    }
}
