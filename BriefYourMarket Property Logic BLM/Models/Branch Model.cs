namespace BriefYourMarketPropertyLogicBLM.Models
{
    internal class BranchModel
    {
        public string Id { get; set; }
        public List<PropertyDataModel> Properties { get; set; } = new();
    }
}
