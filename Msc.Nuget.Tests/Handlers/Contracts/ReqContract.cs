using System;
using System.Collections.Generic;
using System.Text;
using Msc.Microservice.Layer.RabbitMq;

namespace Msc.Nuget.Tests.Handlers.Contracts
{
    [RabbitContract("MytestConstract")]
    public class ReqContract
    {
        public int Id { get; set; }

        public string TestSrt { get; set; }

        public ReqContract ReqContractValue { get; set; }
    }
}
