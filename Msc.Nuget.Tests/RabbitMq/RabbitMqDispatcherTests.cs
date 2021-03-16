using System.Text;
using Msc.Microservice.Layer.RabbitMq;
using Msc.Microservice.Layer.RabbitMq.Dispatcher;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Msc.Nuget.Tests.RabbitMq
{
    [TestFixture]
    public class RabbitMqDispatcherTests
    {
        public IMsgDispatcher _dispatcher;

        public IMessageSerializer _seri = new OwJsonSerializer(null, Mock.Of<ILogger<IMessageSerializer>>());

        public RabbitMqDispatcherTests()
        {

        }

        [Order(0)]
        [Test]
        public void TestDispatchContractType()
        {
            _dispatcher = new MessageDispatcher(Mock.Of<ILogger<IMsgDispatcher>>(), new[]
            {
                new Testhandler(),
            });

            Assert.IsNotNull(_dispatcher.TryGetByAlias("SomeContract"));
        }

        [Order(1)]
        [Test]
        public void TestSerializeType()
        {
            var testContract = new TestContract() { SomeData = "THEDATA" };
            var message = new OwMessageCommon("SomeContract");

            var jsonMessage = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(jsonMessage);
            var resultMsg = _seri.DeserializeTransferMessage(bytes, out var type, _dispatcher.TryGetByAlias) as TestContract;
            Assert.IsNotNull(resultMsg);
            Assert.AreEqual(testContract.SomeData, resultMsg.SomeData);
        }
    }
}
