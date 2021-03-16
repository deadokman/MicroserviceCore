using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Nuget.Tests.Handlers.Contracts;

namespace Msc.Nuget.Tests.Handlers
{
    public class DefaultHandlerMock : IMessageHandler<ReqContract>
    {
        public bool HandlerHasBeenCalled = false;

        public DefaultHandlerMock()
        {
            HandlerHasBeenCalled = true;
        }

        public void HandleMessage(IRmqMessageWrap<ReqContract> msg)
        {
            RabbitMqMessageHandlerIntegration.ReciverSyncObject.RecivedContract = msg.Payload;
            msg.Ack();
            RabbitMqMessageHandlerIntegration.ReciverSyncObject.Mrs.Set();
        }
    }
}
