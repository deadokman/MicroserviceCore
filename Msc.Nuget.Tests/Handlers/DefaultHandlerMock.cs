using System;
using System.Collections.Generic;
using System.Text;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Nuget.Tests.Handlers.Contracts;
using Moq;
using NUnit.Framework;

namespace Msc.Nuget.Tests.Handlers
{
    public class DefaultHandlerMock : IMessageHandler<ReqContract>
    {
        public bool HandlerHasBeenCalled = false;

        public DefaultHandlerMock()
        {
            HandlerHasBeenCalled = true;
        }

        public void HandleMessage(IRmqMessageWrap<ReqContract> msg)
        {
            RabbitMqMessageHandlerIntegration.ReciverSyncObject.RecivedContract = msg.Payload;
            msg.Ack();
            RabbitMqMessageHandlerIntegration.ReciverSyncObject.Mrs.Set();
        }
    }
}
