using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Msc.Microservice.Layer.GrpcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProxyInfrastructure;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Core.Standalone.Implementations;

namespace Msc.Microservice.GrpcTest
{
    [TestFixture]
    public class ClientProxyLayerTest
    {
        public IMicroserviceCore Mc;

        [SetUp]
        public void SetUp()
        {
            ClientCache.Instance.Add<ProxiM.ProxiMClient>();
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
            var client = clientService.GetInstance<ProxiM.ProxiMClient>();
            Assert.IsNotNull(client);

            Assert.DoesNotThrow(() =>
            {
                var tasks = new Task[10];
                for (var i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        var rootPath = TestContext.CurrentContext.TestDirectory;
                        var well = File.OpenRead(Path.Combine(rootPath, "files", "main_05.02.xlsx"));
                        var model = File.OpenRead(Path.Combine(rootPath, "files", "sub_05.02.xlsx"));
                        var id = Guid.NewGuid();
                        var reply = client.Execute(new CalcRequest
                        {
                            Id = ByteString.CopyFrom(id.ToByteArray()),
                            WellDataFile = ByteString.FromStream(well),
                            ModelFile = ByteString.FromStream(model),
                        });

                        for (var index = 0; index < reply.Files.Count; index++)
                        {
                            var calcFile = reply.Files[index];
                            var file = File.Open(Path.Combine(rootPath, $"out_{index}_{id}.xlsx"), FileMode.OpenOrCreate);
                            calcFile.Content.WriteTo(file);
                            file.Close();
                        }

                        Assert.NotNull(reply);
                    }, stopservicetoken);
                }

                Task.WaitAll(tasks);
            });

            Mc.Stop();
        }

        [Test]
        public void TestClientLayer()
        {
            Mc.Run();
        }
    }
}
