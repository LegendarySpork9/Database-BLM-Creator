using BriefYourMarketPropertyLogicBLM.Models;
using BriefYourMarketPropertyLogicBLM.Services;

namespace BriefYourMarketPropertyLogicBLM.Functions
{
    internal class ApplicationFunction
    {
        public string GetFunction(string[] args)
        {
            string command = "";

            foreach (string arg in args)
            {
                command += $"{arg} ";
            }

            return command.Trim();
        }

        public List<InstanceModel> SetupInstanceModel(string instanceList)
        {
            DatabaseService _databaseService = new(new LoggerService("System"));

            List<InstanceModel> instances = new();
            string[] instanceArray = instanceList.Split(',');

            foreach (string instance in instanceArray)
            {
                (int instanceId, string databaseServer, string database, string branchIds) = _databaseService.GetInstanceConfig(instance);

                string[] branchIdList = branchIds.Split(',');
                List<BranchModel> branches = new();
                
                foreach (string branchId in branchIdList)
                {
                    BranchModel branch = new()
                    {
                        Id = branchId
                    };

                    branches.Add(branch);
                }

                InstanceModel instanceModel = new()
                {
                    Id = instanceId,
                    Domain = instance,
                    DatabaseServer = databaseServer,
                    Database = database,
                    Branches = branches
                };

                instances.Add(instanceModel);
            }

            return instances;
        }
    }
}
