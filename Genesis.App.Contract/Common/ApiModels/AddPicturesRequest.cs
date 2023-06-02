namespace Genesis.App.Contract.Common.ApiModels
{
    public class AddPicturesRequest
    {
        public IEnumerable<NewPictureRequest> Pictures { get; set; }
    }
}
