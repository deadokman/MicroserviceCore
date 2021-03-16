using NUnit.Framework;
using System;
using System.Threading;
using Msc.Microservice.Core.Standalone.Implementations;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Microservice.Layer.RabbitMq.Layer;
using Msc.Nuget.Tests.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Msc.Nuget.Tests
{
    [TestFixture]
    public class RabbitLayerTest
    {
        public IMicroserviceCore Mc;

        [SetUp]
        public void SetUp()
        {
            Mc = new MicroserviceCore();
            Mc.DoWork += McOnDoWork;
            Mc.AddLayer(new RabbitMqLayer(true, typeof(DefaultHandlerMock), typeof(RpcHandlerMock)));
            Mc.RegisterConfigurations += McOnRegisterConfigurations;
        }

        private void McOnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            e.AddJsonFile("./TestConfig.json");
        }

        private void McOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var rabbit = serviceprovider.GetService<IMessageQueueClient>();
            Assert.IsNotNull(rabbit);
            Mc.Stop();
            
        }

        [Test]
        public void TestRabbitLayer()
        {
            Mc.Run();
        }
    }
}
