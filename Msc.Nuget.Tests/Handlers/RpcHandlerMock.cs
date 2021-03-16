using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Nuget.Tests.Handlers.Contracts;

namespace Msc.Nuget.Tests.Handlers
{
    public class RpcHandlerMock : IRpcMessageHandler<ReqContract, RespContract>
    {
        public RespContract HandleRpc(IRmqMessageWrap<ReqContract> arg)
        {
            return new RespContract() { Id = 1, Name = "VALUE" };
        }
    }
}
