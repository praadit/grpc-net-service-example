using System.Runtime.Serialization;

namespace GrpcContracts.Messages.Request
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember(Order = 1)]        
        public string Token { get; set; }
    }
}