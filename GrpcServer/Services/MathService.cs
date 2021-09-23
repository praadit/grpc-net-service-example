using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer
{
    public class MathService : Math.MathBase
    {
        private readonly ILogger<GreeterService> _logger;
        public MathService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }
        public override Task<MathReply> Addition(MathRequest request, ServerCallContext context){
            return Task.FromResult(new MathReply
            {
                Summary = request.Number1 + request.Number2
            });
        }

        public override async Task Increment(IncrementRequest request, IServerStreamWriter<IncrementResponse> responseStream,  ServerCallContext context){
            for (int i = 0; i < request.Limit; i++)
            {
                await responseStream.WriteAsync(new IncrementResponse{
                    Result = request.Start + i,
                });
                await Task.Delay(1000);
            }
        }
    }
}