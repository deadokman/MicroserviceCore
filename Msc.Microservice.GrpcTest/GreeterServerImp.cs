using System.Threading.Tasks;
using Com.Example.Grpc;
using Grpc.Core;

namespace Msc.Microservice.GrpcTest
{
    public class GreeterServerImp : GreetingService.GreetingServiceBase
    {
        public override async Task<HelloResponse> greetingNew(GreetingEmpty request, ServerCallContext context)
        {
            return await Task.FromResult(new HelloResponse());
        }

        public override Task<HelloResponse> greeting(HelloRequest request, ServerCallContext context)
        {
            return new Task<HelloResponse>(() => new HelloResponse());
        }
    }
}