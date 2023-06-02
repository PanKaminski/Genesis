namespace Genesis.App.Contract.Common.ApiModels
{
    public class PictureResponse
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string PublicId { get; set; }

        public bool IsMain { get; set; }

        public string DataAdded { get; set; }
    }
}
