namespace BriefYourMarketPropertyLogicBLM.Models
{
    internal class PropertyDataModel
    {
        public int Id { get; set; }
        public List<DataModel> Data { get; set; } = new();
        public List<ImageDataModel> Images { get; set; } = new();
    }
}
