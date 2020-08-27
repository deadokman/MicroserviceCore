using System;
using System.Threading;

using Msc.Microservice.Core.Standalone.Implementations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Msc.Microservice.Layer.Redis;

using NUnit.Framework;

namespace Msc.Nuget.Tests.RedisTest
{
    [TestFixture]
    public class RedisLayerTest
    {
        private MicroserviceCore _service;

        private string _redisKey = "SomeKey";

        private string _redistestVal = "someval";

        private IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            _service = new MicroserviceCore();
            _service.RegisterConfigurations += _service_OnRegisterConfigurations;
            _service.AddLayer(new RedisAccessLayer());
            _service.DoWork += _service_OnDoWork;
        }

        private void _service_OnDoWork(IServiceProvider sp, CancellationToken token)
        {
            _serviceProvider = sp;
            var cache = sp.GetService<IDistributedCache>();
            cache.SetString(_redisKey, _redistestVal);
            _service.Stop();
        }

        private void _service_OnRegisterConfigurations(object? sender, IConfigurationBuilder e)
        {
            e.AddJsonFile("./TestConfig.json");
        }

        [Test]
        public void TestRedisConnect()
        {
            _service.Run();
            var cache = _serviceProvider.GetService<IDistributedCache>();
            var cachedData = cache.GetString(_redisKey);
            Assert.AreEqual(_redistestVal, cachedData);
        }

        [Test]
        public void TestNoData()
        {
            _service.Run();
            var cache = _serviceProvider.GetService<IDistributedCache>();
            var cachedData = cache.GetString("wrongkey");
        }
    }
}
