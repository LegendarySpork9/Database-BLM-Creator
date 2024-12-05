using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Functions;
using BriefYourMarketPropertyLogicBLM.Models;

namespace BriefYourMarketPropertyLogicBLM.Services
{
    internal class ApplicationService
    {
        public void RunApplication(string[] args)
        {
            LoggerService _logger = new("System");
            ApplicationFunction _applicationFunction = new();

            if (args.Length != 0)
            {
                string command = _applicationFunction.GetFunction(args);

                if (command.ToLower() == "-test")
                {
                    TestBLM();
                }

                else if (command.ToLower().Contains("-test "))
                {
                    if (AppSettingsModel.Instances.Contains(args[1]))
                    {
                        if (args.Length == 2)
                        {
                            TestBLM(args[1]);
                        }

                        else
                        {
                            TestBLM(args[1], args[2]);
                        }
                    }

                    else
                    {
                        _logger.LogMessage(StandardValues.LoggerValues.Warning, $"Instance {args[1]} is not Specified in App Settings");
                    }
                }

                else if (command.ToLower() == "-run")
                {
                    RunBLM();
                }

                else if (command.ToLower().Contains("-run "))
                {
                    if (AppSettingsModel.Instances.Contains(args[1]))
                    {
                        if (args.Length == 2)
                        {
                            RunBLM(args[1]);
                        }

                        else
                        {
                            RunBLM(args[1], args[2]);
                        }
                    }

                    else
                    {
                        _logger.LogMessage(StandardValues.LoggerValues.Warning, $"Instance {args[1]} is not Specified in App Settings");
                    }
                }

                else
                {
                    _logger.LogMessage(StandardValues.LoggerValues.Info, "The command is not recognised, refer to the help document for commands.");
                    DisplayHelp();
                }
            }

            else
            {
                _logger.LogMessage(StandardValues.LoggerValues.Info, "Running all instances and branches");

                RunBLM();
            }
        }

        public void TestBLM(string? instance = null, string? branchId = null)
        {
            ApplicationFunction _applicationFunction = new();
            FTPService _ftpService = new();

            List<InstanceModel> instances = _applicationFunction.SetupInstanceModel(instance ?? AppSettingsModel.Instances);

            foreach (InstanceModel instanceModel in instances)
            {
                LoggerService _logger = new(instanceModel.Domain.Replace(".briefyourmarket.com", ""));
                DatabaseService _databaseService = new(_logger);

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    foreach (BranchModel branch in instanceModel.Branches)
                    {
                        _databaseService.GetPropertyData(instanceModel, branch);
                    }

                    _logger.LogMessage(StandardValues.LoggerValues.Debug, $"{instanceModel.Branches.Count} BLM files would be created");
                }

                else
                {
                    _databaseService.GetPropertyData(instanceModel, instanceModel.Branches[instanceModel.Branches.FindIndex(a => a.Id == branchId)]);
                    _logger.LogMessage(StandardValues.LoggerValues.Debug, "1 BLM file would be created");
                }
            }

            _ftpService.TestConnection();
        }

        public void RunBLM(string? instance = null, string? branchId = null)
        {
            ApplicationFunction _applicationFunction = new();
            FTPService _ftpService = new();

            List<InstanceModel> instances = _applicationFunction.SetupInstanceModel(instance ?? AppSettingsModel.Instances);

            foreach (InstanceModel instanceModel in instances)
            {
                LoggerService _logger = new(instanceModel.Domain.Replace(".briefyourmarket.com", ""));
                DatabaseService _databaseService = new(_logger);
                DocumentService _documentService = new(_logger);

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    foreach (BranchModel branch in instanceModel.Branches)
                    {
                        _databaseService.GetPropertyData(instanceModel, branch);
                        (string blmPath, string blmName, bool success) = _documentService.CreateBLM(instanceModel.Domain, branch);

                        if (success)
                        {
                            _documentService.CreateZIP(blmPath);
                            _ftpService.UploadZip(_logger, blmPath.Replace(".blm", ".zip"), blmName.Replace(".blm", ".zip"));
                        }
                    }
                }

                else
                {
                    _databaseService.GetPropertyData(instanceModel, instanceModel.Branches[instanceModel.Branches.FindIndex(a => a.Id == branchId)]);
                    (string blmPath, string blmName, bool success) = _documentService.CreateBLM(instanceModel.Domain, instanceModel.Branches[instanceModel.Branches.FindIndex(a => a.Id == branchId)]);

                    if (success)
                    {
                        _documentService.CreateZIP(blmPath);
                        _ftpService.UploadZip(_logger, blmPath.Replace(".blm", ".zip"), blmName.Replace(".blm", ".zip"));
                    }
                }
            }
        }

        public void DisplayHelp()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Option    Description");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("blank    Runs all instances & branches");
            Console.WriteLine("Test All (-test)    Tests all the instances & branches");
            Console.WriteLine("Test Instance (-test url)    Tests the instance matching the URL");
            Console.WriteLine("Test Instance Branch (-test url branchId)    Tests the BLM matching the url and branch id");
            Console.WriteLine("Run All (-run)    Runs all the instances & branches");
            Console.WriteLine("Run Instance (-run url)    Runs the instance matching the URL");
            Console.WriteLine("Run Instance Branch (-run url branchId)    Runs the BLM matching the url and branch id");
            Console.WriteLine();
        }
    }
}
