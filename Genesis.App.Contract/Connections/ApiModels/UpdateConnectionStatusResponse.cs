using Genesis.App.Contract.Models.Forms;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Connections.ApiModels
{
    public class UpdateConnectionStatusResponse
    {
        public int UserId { get; set; }

        public int? ConnectionId { get; set; }

        public ConnectionStatus? Status { get; set; }

        public IEnumerable<Button> Buttons { get; set; }
    }
}
