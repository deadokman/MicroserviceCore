using System.Collections.Generic;
using System.IO;
using Com.Example.Grpc;
using Grpc.Core;
using NUnit.Framework;

namespace Msc.Microservice.GrpcTest
{
    [TestFixture]
    public class ClientTest
    {
        private const string Folder = @"..\Msc.Microservice.GrpcTest\certs\2\";

        [Test]
        public void TestClient()
        {
            var cacert = File.ReadAllText(Path.Combine(Folder, "ca.pem"));
            var clientcert = File.ReadAllText(Path.Combine(Folder, "client.pem"));
            var clientkey = File.ReadAllText(Path.Combine(Folder, "client.key"));
            var ssl = new SslCredentials(cacert, new KeyCertificatePair(clientcert, clientkey), context =>
                {
                    return true;
                });

            var options = new List<ChannelOption>
            {
                //new ChannelOption(ChannelOptions.SslTargetNameOverride, "foo.test.google.fr")
                new ChannelOption(ChannelOptions.SslTargetNameOverride, "KONYSHEVS-PC")
                //konyshevs
            };

            //var channel = new Channel("188.68.208.122", 555, clientCredentials, options);
            //var channel = new Channel("frontend.local", 4443, ssl, options);
            var channel = new Channel("localhost", 555, ssl, options);
            var client = new GreetingService.GreetingServiceClient(channel);

            var re = client.greetingNew(new GreetingEmpty());
        }

        [Test]
        public void TestServer()
        {
            var rootCert = File.ReadAllText($"{Folder}ca.pem");
            var keyCertPair = new KeyCertificatePair(
                File.ReadAllText($"{Folder}server0.pem"),
                File.ReadAllText($"{Folder}server0.key"));
            var serverCredentials = new SslServerCredentials(new[] { keyCertPair }, rootCert, SslClientCertificateRequestType.DontRequest);

            var server = new Server(new[] { new ChannelOption(ChannelOptions.SoReuseport, 0) })
            {
                Ports = { new ServerPort("localhost", 555, serverCredentials) },
            };

            server.Services.Add(GreetingService.BindService(new GreeterServerImp()));
            server.Start();
        }
    }
}