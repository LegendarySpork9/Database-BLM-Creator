namespace BriefYourMarketPropertyLogicBLM.Models
{
    internal class InstanceModel
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public string DatabaseServer { get; set; }
        public string Database { get; set; }
        public List<BranchModel> Branches { get; set; } = new();
    }
}
