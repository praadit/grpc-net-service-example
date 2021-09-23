using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:6001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest{Name = "I'am Client"});
            Console.WriteLine("Greeting : "+ reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            var mathClient = new Math.MathClient(channel);
            var mathReply = await mathClient.AdditionAsync(new MathRequest{Number1 = 1, Number2 = 2});            
            Console.WriteLine("1 + 2 = "+ mathReply.Summary);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            
            using var incrementCall = mathClient.Increment(new IncrementRequest{Start = 1, Limit = 8});
            await foreach(var response in incrementCall.ResponseStream.ReadAllAsync()){
                Console.WriteLine(response.Result);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
