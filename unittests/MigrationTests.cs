using System;
using WireMock.Server;
using WireMock.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using NUnit.Framework;
using Bisk.Migration;
using Bisk.Messages;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace tests
{
    [TestFixture]
    public class MigrationTests
    {
        private FluentMockServer _server;
        private readonly DisciplineSelected _message = new DisciplineSelected{
                ApplicationId=Guid.NewGuid(),
                DisciplineId = Guid.NewGuid()
            };

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

        [TestCase(TestName = "When receiving DisciplineSelected should call BOAS API")]
        public async Task should_call_boas_api()
        {
            InstrumentServer();

            var sut = new DisciplineSelectedHandler();
            await sut.Handles(string.Empty, _message);

            Assert.AreEqual(1, _server.LogEntries.Count());
        }

        [TestCase(TestName = "When receiving DisciplineSelected should execute HTTP POST to BOAS API")]
        public async Task should_post_to_boas_api()
        {
            InstrumentServer();

            var sut = new DisciplineSelectedHandler();
            await sut.Handles(string.Empty, _message);

            Assert.AreEqual(1, _server.FindLogEntries(Request.Create().UsingPost()).Count());
        }

        [TestCase(TestName = "When receiving DisciplineSelected should send payload via HTTP POST to BOAS API")]
        public async Task should_post_payload_to_boas_api()
        {
            InstrumentServer();

            var sut = new DisciplineSelectedHandler();
            await sut.Handles(string.Empty, _message);

            var payload = new {
                ApplicationId = _message.ApplicationId,
                DsciplineId = _message.DisciplineId
            };
            var body = JsonSerializer.Serialize(payload);
            var matcher = Request.Create()
                            .UsingPost()
                            .WithBody(body);
            Assert.AreEqual(1, _server.FindLogEntries(matcher).Count());
        }
    }
}
