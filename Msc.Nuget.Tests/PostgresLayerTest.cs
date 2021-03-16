using System;
using System.Threading;
using Msc.Interface.Db;
using Msc.Microservice.Core.Standalone.Implementations;
using Msc.Microservice.Layer.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Msc.Nuget.Tests
{
    [TestFixture]
    public class PostgresLayerTest
    {
        private MicroserviceCore Mc;

        [SetUp]
        public void SetUpData()
        {
            Environment.SetEnvironmentVariable("ms_Postgres__ConnectionString", "value");
            Mc = new MicroserviceCore();
            Mc.DoWork += McOnDoWork;
            Mc.AddLayer(new PostgresAccessLayer());
        }

        private void McOnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            //e.AddEnvironmentVariables("ms_");
        }


        private void McOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var dbConext = serviceprovider.GetService<IDbContext>();
            Assert.IsNotNull(dbConext);
            Mc.Stop();
        }

        [Order(1)]
        [Test]
        public void TestPostgresConfigWithConfiguration()
        {
            Mc.RegisterConfigurations += McOnRegisterConfigurations;
            Mc.Run();
        }
    }
}
