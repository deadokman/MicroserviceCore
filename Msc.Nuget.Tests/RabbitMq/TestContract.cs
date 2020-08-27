using System;
using System.Collections.Generic;
using System.Text;
using Msc.Microservice.Layer.RabbitMq;

namespace Msc.Nuget.Tests.RabbitMq
{
    [RabbitContract("SomeContract")]
    public class TestContract
    {
        public string SomeData { get; set; }
    }
}
