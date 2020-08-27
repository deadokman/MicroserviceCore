using System;
using System.Collections.Generic;
using System.Text;
using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Nuget.Tests.Handlers.Contracts
{
    public class RespContract 
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
