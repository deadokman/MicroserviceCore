using System;
using System.Collections.Generic;
using System.Threading;
using Msc.Microservice.Core.Standalone.Implementations;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Nuget.Tests.Microservice;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using IServiceProvider = System.IServiceProvider;

namespace Msc.Nuget.Tests
{
    public class Tests
    {
        MicroserviceCore mc = new TestTargetMc();

        private Dictionary<string, bool> _methodCallTestObject;
        private Mock<IMicroserviceLayer> _mcLayer;

        [SetUp]
        public void Setup()
        {
            mc.DoWork += McOnDoWork;
            mc.PrepareExecution += McOnPrepareExecution;
            mc.PrepareShutdown += McOnPrepareShutdown;
            mc.RegisterConfigurations += McOnRegisterConfigurations;
            _methodCallTestObject = new Dictionary<string, bool>();
            _mcLayer = new Mock<IMicroserviceLayer>();

            _mcLayer.SetupAllProperties();
            mc.AddLayer(_mcLayer.Object);
        }

        private void McOnRegisterConfigurations(object sender, IConfigurationBuilder e)
        {
            _methodCallTestObject[nameof(McOnRegisterConfigurations)] = true;
        }


        private void McOnPrepareShutdown(IServiceProvider sp)
        {
            _methodCallTestObject[nameof(McOnPrepareShutdown)] = true;
        }

        private void McOnPrepareExecution(IServiceCollection servicecollection, IConfigurationRoot configuration)
        {
            _methodCallTestObject[nameof(McOnPrepareExecution)] = true;
        }

        private void McOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            _methodCallTestObject[nameof(McOnDoWork)] = true;
            Thread.Sleep(1000);
            mc.Stop();
        }

        [Test]
        public void RunServiceBgTest()
        {
            mc.Run();
            Assert.IsTrue(_methodCallTestObject[nameof(McOnPrepareShutdown)]);
            Assert.IsTrue(_methodCallTestObject[nameof(McOnPrepareExecution)]);
            Assert.IsTrue(_methodCallTestObject[nameof(McOnDoWork)]);
            Assert.IsTrue(_methodCallTestObject[nameof(McOnRegisterConfigurations)]);
            _mcLayer.Verify(m => m.RegisterConfiguration(It.IsAny<IConfigurationBuilder>()), Times.Once);
            _mcLayer.Verify(m => m.RegisterLayer(It.IsAny<IConfigurationRoot>(), It.IsAny<IServiceCollection>()), Times.Once);
        }
    }
}