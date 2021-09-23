using System.Threading;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;
using ProtoBuf.Grpc.Client;
using GrpcContracts.Services.Interfaces;
using GrpcContracts.Messages.Request;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Program program = new Program();
            using var channel = GrpcChannel.ForAddress("https://localhost:6001");
            
            await program.AsyncUnaryCall(channel);
            await program.AsyncServerStreamingCall(channel);
            await program.AsyncClientStreamCall(channel);
            await program.AsyncChatService(channel);
            await program.ProtobufIntegration(channel);
        }

        public async Task AsyncUnaryCall(GrpcChannel channel){
            var mathClient = new Math.MathClient(channel);
            var client = new Greeter.GreeterClient(channel);

            var reply = await client.SayHelloAsync(new HelloRequest{Name = "I'am Client"});
            Console.WriteLine("Greeting : "+ reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            var mathReply = await mathClient.AdditionAsync(new MathRequest{Number1 = 1, Number2 = 2});            
            Console.WriteLine("1 + 2 = "+ mathReply.Summary);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public async Task AsyncServerStreamingCall(GrpcChannel channel){            
            var mathClient = new Math.MathClient(channel);

            using var incrementCall = mathClient.Increment(new IncrementRequest{Start = 1, Limit = 8});
            await foreach(var response in incrementCall.ResponseStream.ReadAllAsync()){
                Console.WriteLine(response.Result);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public async Task AsyncClientStreamCall(GrpcChannel channel){           
            var mathClient = new Math.MathClient(channel);

            using var clientStreamCall = mathClient.SummaryStream();
            for (int i = 0; i < 3; i++)
            {
                await clientStreamCall.RequestStream.WriteAsync(new SummaryRequest{
                    Addition = 2,
                });
            }
            await clientStreamCall.RequestStream.CompleteAsync();
            var res = await clientStreamCall;
            foreach(var result in res.Summary){
                Console.WriteLine(result.Result);
            }
        }

        public async Task AsyncChatService(GrpcChannel channel){
            var client = new Chat.ChatClient(channel);
            using var call = client.SendMessage();

            var tokenResource = new CancellationTokenSource();
            
            var readTask = Task.Run(async () => {
                await foreach(var response in call.ResponseStream.ReadAllAsync(tokenResource.Token)){
                    Console.WriteLine(response.Message);
                }
            });

            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    tokenResource.Cancel();
                    break;
                }

                await call.RequestStream.WriteAsync(new Client2Server { Message = result });
            }

            Console.WriteLine("Disconnecting");
            await call.RequestStream.CompleteAsync();
            try{
                await readTask;
            }catch(RpcException e) when (e.Status.StatusCode == StatusCode.Cancelled){                
                Console.WriteLine("Client Request cancelation");
            }
        }
        
        public async Task ProtobufIntegration(GrpcChannel channel){
            var client = channel.CreateGrpcService<ILoginService>();

            var reply = await client.Login(new LoginRequest { Token = "ajhsdkahwdhashdkawd" });

            Console.WriteLine($"GameToken : {reply.LoginData.GameToken}");
        }
    }
}
