namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class TreesListResponse
    {
        public ICollection<GenealogicalTreeItemResponse> Trees { get; set; } = new List<GenealogicalTreeItemResponse>();

        public void Add(int id, int personsCount, string name, DateTime lastModified, bool isOwned = false)
        {
            Trees.Add(new GenealogicalTreeItemResponse
            {
                Id = id,
                Name = name,
                PersonsCount = personsCount,
                IsOwned = isOwned,
                LastUpdate = lastModified.ToString("MM/dd/yyyy h:mm tt"),
            });
        }
    }
}
