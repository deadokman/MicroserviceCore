using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Nuget.Tests.RabbitMq
{
    public class Testhandler : IMessageHandler<TestContract>
    {
        public void HandleMessage(IRmqMessageWrap<TestContract> msg)
        {
            return;;
        }
    }
}
