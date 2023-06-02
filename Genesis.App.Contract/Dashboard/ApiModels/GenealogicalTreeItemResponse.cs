namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class GenealogicalTreeItemResponse
    {
        public int Id { get; set; }
        public bool IsOwned { get; set; }
        public int PersonsCount { get; set; }
        public string Name { get; set; }
        public string LastUpdate { get; set; }
    }
}
