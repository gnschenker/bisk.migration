using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using WireMock.Server;
using WireMock.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using NUnit.Framework;
using Bisk.Messaging;
using Bisk.Messages;
using NUnit.Framework.Constraints;

namespace integrationtests
{
    [TestFixture]
    public class MigrationTests
    {
        private FluentMockServer _server;
        private readonly DisciplineSelected _message = new DisciplineSelected{
                ApplicationId=Guid.NewGuid(),
                DisciplineId = Guid.NewGuid()
            };
        IResolveConstraint constraint = Is.True.After(delayInMilliseconds: 5000, pollingInterval: 100);

        [SetUp]
        public void Setup()
        {
            _server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Port = 5000
            });
        }

        [TearDown]
        public void Teardown()
        {
            _server.Stop();
        }

        private void InstrumentServer()
        {
            _server
                .Given(
                    Request.Create().WithPath("/api/disciplines").UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(201)
                );
        }

        private void SendMessage()
        {
            var publisher = new RabbitMqPublisherStrategy();
            publisher.Publish(string.Empty, _message);
        }

        [TestCase(TestName = "When receiving DisciplineSelected should call BOAS API")]
        public void should_call_boas_api()
        {
            InstrumentServer();

            SendMessage();

            Assert.That(() => _server.LogEntries.Count() == 1, constraint);
        }

        [TestCase(TestName = "When receiving DisciplineSelected should execute HTTP POST to BOAS API")]
        public void should_post_to_boas_api()
        {
            InstrumentServer();

            SendMessage();

            Assert.That(() => _server.FindLogEntries(Request.Create().UsingPost()).Count() == 1, constraint);
        }

        [TestCase(TestName = "When receiving DisciplineSelected should send payload via HTTP POST to BOAS API")]
        public void should_post_payload_to_boas_api()
        {
            InstrumentServer();

            SendMessage();

            var payload = new {
                ApplicationId = _message.ApplicationId,
                DsciplineId = _message.DisciplineId
            };
            var body = JsonSerializer.Serialize(payload);
            var matcher = Request.Create()
                            .UsingPost()
                            .WithBody(body);
            Assert.That(() => _server.FindLogEntries(matcher).Count() == 1, constraint);
        }

    }
}