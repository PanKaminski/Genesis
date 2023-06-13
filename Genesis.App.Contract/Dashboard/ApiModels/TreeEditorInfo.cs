using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class TreeEditorInfo
    {
        public int TreeId { get; set; }

        public IEnumerable<ControlValue> Values { get; set; }

        public SavePictureRequest Picture { get; set; }
    }
}
