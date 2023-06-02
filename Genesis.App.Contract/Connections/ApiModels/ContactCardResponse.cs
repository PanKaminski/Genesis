using Genesis.Common.Enums;

namespace Genesis.App.Contract.Connections.ApiModels
{
    public class ContactCardResponse : UserCardResponse
    {
        public int ConnectionId { get; set; }

        public ConnectionStatus Status { get; set; }
    }
}
