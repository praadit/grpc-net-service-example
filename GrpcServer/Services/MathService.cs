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

        public override async Task<SummaryResponse> SummaryStream(IAsyncStreamReader<SummaryRequest> requestStream, ServerCallContext context){
            int start = 1;
            var temp = start;
            var response = new SummaryResponse{
                Summary = {}
            };
            await foreach(var request in requestStream.ReadAllAsync()){
                temp += request.Addition;
                response.Summary.Add(new SummaryData{
                    Addition = request.Addition,
                    Result = temp,
                    Start = temp - request.Addition
                });
            }

            return response;
        }
    }
}