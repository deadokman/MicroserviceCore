using System;
using System.Threading;
using System.Threading.Tasks;
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
            _service.DoWork += ServiceOnDoWork;
        }

        private void ServiceOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            _serviceProvider = serviceprovider;
            _service.Stop();
        }

        private void _service_OnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            e.AddJsonFile("./TestConfig.json");
        }

        [Test]
        public void TestRedisConnect()
        {
            _service.Run();
            var cache = _serviceProvider.GetRequiredService<IExtendDistributedCache>();
            var cachedData = cache.GetString(_redisKey);
            Assert.AreEqual(_redistestVal, cachedData);
        }

        [Test]
        public async Task TestHashSetCreate()
        {
            _service.Run();
            var cache = _serviceProvider.GetRequiredService<IExtendDistributedCache>();
            await cache.SetHValuesAsync("test_hash_key", ("a1", "a2"), ("a2", "a3"));
            var val = await cache.GetHValueAsync("test_hash_key", "a2");
            Assert.AreEqual("a3", val);
        }

        [Test]
        public async Task TestHashAddAndRemove()
        {
            _service.Run();
            var cache = _serviceProvider.GetRequiredService<IExtendDistributedCache>();
            await cache.SetHValuesAsync("test_hash_key", ("a1", "a2"), ("a2", "a3"));
            var val = await cache.GetHValueAsync("test_hash_key", "a2");
            Assert.AreEqual("a3", val);
            await cache.DelHAsync("test_hash_key");
            var val2 = await cache.GetHValueAsync("test_hash_key", "a2");
            Assert.IsTrue(String.IsNullOrEmpty(val2));
        }

        [Test]
        public void TestNoData()
        {
            _service.Run();
            var cache = _serviceProvider.GetRequiredService<IExtendDistributedCache>();
            var cachedData = cache.GetString("wrongkey");
        }

        [Test]
        public void TestRedisIncrConnect()
        {
            _service.DoWork += (sp, token) =>
            {
                var cache = sp.GetRequiredService<IExtendDistributedCache>();

                cache.ResetIncr(_redisKey, 33);
                cache.SetIncr(_redisKey);

                var incr = cache.GetIncr(_redisKey);
                Assert.AreEqual(incr, 34);
                _service.Stop();
            };

            _service.Run();
        }
    }
}
