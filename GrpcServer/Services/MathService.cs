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
            _logger.LogInformation("masuk");

            return Task.FromResult(new MathReply
            {
                Summary = request.Number1 + request.Number2
            });
        }
    }
}