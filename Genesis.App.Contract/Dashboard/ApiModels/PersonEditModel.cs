using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class PersonEditModel
    {
        public PersonFormParams PersonEditorInfo { get; set; }

        public IEnumerable<ControlValue> FormValues { get; set; }

        public IEnumerable<EditPictureRequest> UpdatedPhotos { get; set; } = new List<EditPictureRequest>();

        public IEnumerable<int> RemovedPhotos { get; set; } = new List<int>();
        public IEnumerable<SavePictureRequest> AddedPhotos { get; set; } = new List<SavePictureRequest>();
    }
}
