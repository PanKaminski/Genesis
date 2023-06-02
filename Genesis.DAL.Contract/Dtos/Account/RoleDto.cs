using Genesis.Common.Enums;

namespace Genesis.DAL.Contract.Dtos.Account
{
    public class RoleDto
    {
        public int Id { get; set; }

        public Role RoleName { get; set; }

        public IList<AccountDto> Accounts { get; set; }
    }
}
