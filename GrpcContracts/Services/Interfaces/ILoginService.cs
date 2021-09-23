using System.ServiceModel;
using GrpcContracts.Messages.Response;
using GrpcContracts.Messages.Request;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace GrpcContracts.Services.Interfaces
{
    [ServiceContract]
    public interface ILoginService
    {   
        [OperationContract]
        Task<LoginResponse> Login(LoginRequest request, CallContext context = default);
    }
}