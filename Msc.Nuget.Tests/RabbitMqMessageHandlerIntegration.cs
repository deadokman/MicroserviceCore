using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msc.Microservice.Core.Standalone.Implementations;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Layer.Postgres;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Microservice.Layer.RabbitMq.Layer;
using Msc.Nuget.Tests.Handlers;
using Msc.Nuget.Tests.Handlers.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Msc.Nuget.Tests
{

    [TestFixture]
    public class RabbitMqMessageHandlerIntegration
    {
        private IMicroserviceCore _publisherService;
        private IMicroserviceCore _reciverService;
        private IMicroserviceCore _rpcService;

        /// <summary>
        /// Класс синхронизации для проверки того, чтобы произвести проверку того, что был получен объект
        /// </summary>
        public static class ReciverSyncObject
        {
            public static ReqContract RecivedContract;

            public static ManualResetEventSlim Mrs = new ManualResetEventSlim();
        }

        private ReqContract _originalContract;

        [SetUp]
        public void SetUp()
        {
            _publisherService = new MicroserviceCore(configFileName: "TestPostgresLayerConfiguration");
            _publisherService.AddLayer(new RabbitMqLayer(false, null));
            _publisherService.DoWork += PublisherServiceOnDoWork;
            _reciverService = new MicroserviceCore(configFileName: "TestPostgresLayerConfiguration");
            _reciverService.AddLayer(new RabbitMqLayer(true, typeof(DefaultHandlerMock), typeof(RpcHandlerMock)));
            _reciverService.DoWork+= ReciverServiceOnDoWork;
            _originalContract = new ReqContract() { Id = 1, TestSrt = "TEST", ReqContractValue = new ReqContract() { Id = 2, TestSrt = "TEST2" }};

            _rpcService = new MicroserviceCore(configFileName: "TestPostgresLayerConfiguration");
            _rpcService.AddLayer(new RabbitMqLayer(false));
            _rpcService.DoWork += RpcServiceOnDoWork;
        }

        private void RpcServiceOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var client = serviceprovider.GetService<IMessageQueueClient>();
            var t = client.MakeRpcCallAsync<ReqContract, RespContract>(_originalContract, "Test").Result;
            _rpcService.Stop();
        }

        private void ReciverServiceOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            // Ждем, что из DefaultHandlerMock будет снята защелка для продолжения выполнения теста
            ReciverSyncObject.Mrs.Wait(timeout: new TimeSpan(0, 0, 0, 1));
            _reciverService.Stop();
        }

        private void PublisherServiceOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var client = serviceprovider.GetService<IMessageQueueClient>();
            client.PublishMessage("Test", _originalContract);
            _publisherService.Stop();
        }

        [Test]
        public void lTestDefaultRecive()
        {
            ReciverSyncObject.RecivedContract = null;
            var t1 = _publisherService.RunAsync();
            var t2 = _reciverService.RunAsync();
            // var t3 = _rpcService.RunAsync();
            Task.WaitAll(t1, t2);
            Assert.IsNotNull(ReciverSyncObject.RecivedContract);
            Assert.AreEqual(_originalContract.Id, ReciverSyncObject.RecivedContract.Id);
            Assert.AreEqual(_originalContract.TestSrt, ReciverSyncObject.RecivedContract.TestSrt);
            Assert.IsNotNull(ReciverSyncObject.RecivedContract.ReqContractValue);
        }
    }
}
