using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.DAL.Contract.Dtos.Account
{
    public class AccountConnectionDto : AuditableEntity
    {
        public int AccountFromId { get; set; }
        public AccountDto AccountFrom { get; set; }

        public int AccountToId { get; set; }
        public AccountDto AccountTo { get; set; }

        public ConnectionStatus Status { get; set; }
    }
}