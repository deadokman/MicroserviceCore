using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Core.Standalone.Implementations;

namespace Msc.Microservice.GrpcTest
{
    [TestFixture]
    public class ServerLayerTest
    {
        public IMicroserviceCore Mc;

        [SetUp]
        public void SetUp()
        {
            Mc = new MicroserviceCore();
            Mc.DoWork += McOnDoWork;
            // Mc.AddLayer(new GrpcServerAccessLayer((p) => GreetingService.BindService(new GreeterServerImp())));
            Mc.RegisterConfigurations += McOnRegisterConfigurations;
        }

        private void McOnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            e.AddJsonFile("./TestConfiguration.json");
        }

        private void McOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            //Mc.Stop();
        }

        [Test]
        public void TestServerLayer()
        {
            Mc.Run();
        }
    }
}
