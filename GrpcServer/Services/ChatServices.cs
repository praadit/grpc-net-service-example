using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer
{
    public class ChatServices : Chat.ChatBase
    {
        private readonly ILogger _logger;
        public ChatServices(ILogger<ChatServices> logger)
        {
            _logger = logger;
        }
        public override async Task SendMessage(IAsyncStreamReader<Client2Server> requestStream, IServerStreamWriter<Server2Client> responseStream, ServerCallContext context){
            var clientToServer = Client2ServerHandler(requestStream, context);

            var serverToClient = Server2ClientHandler(responseStream, context);
            await Task.WhenAll(clientToServer, serverToClient);
        }
        private async Task Client2ServerHandler(IAsyncStreamReader<Client2Server> requestStream, ServerCallContext context){
            while(await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested){
                var message = requestStream.Current;
                _logger.LogInformation($"Message from client : {message.Message}");
            }
        }

        private async Task Server2ClientHandler(IServerStreamWriter<Server2Client> responseStream, ServerCallContext context){
            var pingCount = 0;
            while(!context.CancellationToken.IsCancellationRequested){
                pingCount++;
                await responseStream.WriteAsync(new Server2Client{
                    Message = $"Server send message : {pingCount}"
                });
                await Task.Delay(1000);
            }
        }
    }
}