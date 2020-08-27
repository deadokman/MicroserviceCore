using System;
using System.Threading;

using Msc.Microservice.Core.Standalone.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Msc.Nuget.Tests.Circulartest
{
    public interface ISomeIfaceA 
    {

    }

    public interface ISomeIfaceB
    {

    }

    public class ImplementA : ISomeIfaceA
    {
        public ImplementA(ISomeIfaceB B)
        {
            
        }
    }

    public class ImplementB : ISomeIfaceB
    {
        public ImplementB(ISomeIfaceA A)
        {

        }
    }


    [TestFixture]
    public class CircularTest
    {

        MicroserviceCore ms = new MicroserviceCore();

        [SetUp]
        public void SetUp()
        {
            ms.PrepareExecution += Ms_OnPrepareExecution;
            ms.DoWork+= Ms_OnDoWork;
        }

        private void Ms_OnPrepareExecution(IServiceCollection servicecollection, IConfigurationRoot configuration)
        {
            servicecollection.AddTransient<ISomeIfaceA, ImplementA>();
            servicecollection.AddTransient<ISomeIfaceB, ImplementB>();
        }

        private void Ms_OnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            var circular = serviceprovider.GetService<ISomeIfaceB>();
            stopservicetoken.WaitHandle.WaitOne();
        }

        [Test]
        public void Test()
        {
           ms.Run();
        }
    }
}
