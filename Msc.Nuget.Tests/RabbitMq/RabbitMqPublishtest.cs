using System;
using System.Collections.Generic;
using System.Text;
using Msc.Microservice.Layer.RabbitMq;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Msc.Nuget.Tests.RabbitMq
{
    [TestFixture]
    public class RabbitMqPublishtest
    {
        public IMessageQueueClient _client;

        public RabbitMqPublishtest()
        {
            var configMock = new Mock<IOptions<QueuesConfig>>();
            configMock.Setup(m => m.Value)
                .Returns(new QueuesConfig());
                 
            _client = new RabbitMqClient(configMock.Object, 
                Mock.Of<IMsgDispatcher>(), Mock.Of<ILogger<IMessageQueueClient>>(), Mock.Of<IMessageSerializer>());
        }

        [Test]
        public void PublishMessageTest()
        {
            var testMessage = new TestContract() { SomeData = "THEDATA" };
            _client.PublishMessage("somequeue", testMessage);
        }
    }
}
