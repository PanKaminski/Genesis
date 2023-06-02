using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Models.Authentication
{
    public class AccountConnection : AuditableEntity
    {
        public int AccountFromId { get; set; }
        public Account AccountFrom { get; set; }

        public int AccountToId { get; set; }
        public Account AccountTo { get; set; }

        public ConnectionStatus Status { get; set; }
    }
}
