using System.Runtime.Serialization;

namespace GrpcContracts.Messages.Response
{
    [DataContract]
    public class LoginResponse
    {
        [DataMember(Order = 1)]
        public LoginData LoginData { get; set; }
        [DataMember(Order = 2)]
        public long Timestamps { get; set; }
    }

    [DataContract]
    public class LoginData{
        [DataMember(Order = 1)]
        public string GameToken { get; set; }
    }
}