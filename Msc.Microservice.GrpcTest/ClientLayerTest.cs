using System;
using System.Threading;
using Com.Example.Grpc;
using Msc.Microservice.Layer.GrpcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Core.Standalone.Implementations;

namespace Msc.Microservice.GrpcTest
{
    [TestFixture]
    public class ClientLayerTest
    {
        public IMicroserviceCore Mc;

        [SetUp]
        public void SetUp()
        {
            ClientCache.Instance.Add<GreetingService.GreetingServiceClient>();
            Mc = new MicroserviceCore();
            Mc.DoWork += McOnDoWork;
            Mc.AddLayer(new GrpcClientAccessLayer(ClientCache.Instance));
            Mc.RegisterConfigurations += McOnRegisterConfigurations;
        }

        private void McOnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            e.AddJsonFile("./TestConfiguration.json");
        }

        private void McOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var clientService = serviceprovider.GetService<IClientService>();
            var client = clientService.GetInstance<GreetingService.GreetingServiceClient>();
            Assert.IsNotNull(client);

            var reply = client.greetingNew(new GreetingEmpty());

            var client2 = serviceprovider.GetService<GreetingService.GreetingServiceClient>();
            Assert.IsNotNull(client2);

            Mc.Stop();
        }

        [Test]
        public void TestClientLayer()
        {
            Mc.Run();
        }
    }
}
