using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Contract.Connections.ApiModels
{
    public class UserCardResponse
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public int ConnectionsCount { get; set; }

        public string Avatar { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public IEnumerable<Button> Buttons { get; set; }
    }
}
