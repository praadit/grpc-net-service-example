using System;
using GrpcContracts.Services.Interfaces;
using GrpcContracts.Messages.Response;
using GrpcContracts.Messages.Request;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace GrpcServer
{
    public class LoginService : ILoginService
    {
        public Task<LoginResponse> Login(LoginRequest request, CallContext context = default){
            return Task.FromResult(new LoginResponse() {
                LoginData = new LoginData{
                    GameToken = Guid.NewGuid().ToString()
                },
                Timestamps = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            });
        }
    }
}