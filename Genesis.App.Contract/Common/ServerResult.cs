using Genesis.App.Contract.Common.Enums;

namespace Genesis.App.Contract.Common
{
    public class ServerResult
    {
        public ServerResultCode ResultCode { get; set; }

        public string Description { get; set; }

        public ServerResult(ServerResultCode resultCode, string description = null)
        {
            ResultCode = resultCode;
            Description = description;
        }
    }
}
