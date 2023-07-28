using Company.Engine.Registration.Interface;
using Company.iFX.Test;
using Company.Manager.Membership.Interface;
using FluentAssertions;
using Moq;
using ProtoBuf.Grpc;
using Serilog;

namespace Company.Manager.Membership.Impl.Tests
{
    public class MembershipManagerTests
        : IDisposable
    {
        private readonly UnitTestHarness? m_Harness = null;

        public MembershipManagerTests()
        {
            m_Harness = new UnitTestHarness();
            m_Harness.Setup(typeof(MembershipManager));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_Harness != null)
                {
                    m_Harness!.Cleanup();
                }
            }
        }

        [Fact]
        public async Task MembershipManager_GivenRegisterMember()
        {
            var registrationEngineMock = new Mock<IRegistrationEngine>();

            registrationEngineMock
                .Setup(x => x.RegisterAsync(It.IsAny<Engine.Registration.Data.RegisterRequestBase>(), It.IsAny<CallContext>()))
                .Returns(() =>
                {
                    Engine.Registration.Data.RegisterResponseBase response = new Engine.Registration.Data.Web.RegisterResponse
                    {
                        Name = "test",
                        WebMessage = "test"
                    };
                    return Task.FromResult(response);
                });

            var registrationEngineMockService = registrationEngineMock.Object;

            var callerMock = ServiceMock.Create<IMembershipManager>(async proxy =>
            {
                var response = await proxy.RegisterMemberAsync(
                    new Data.Web.RegisterRequest
                    {
                        Name = "test"
                    },
                    default);
                response.Should().NotBeNull();
            });

            //    var trackingContext = new TrackingContext(Guid.Parse("blah"), DateTime.UtcNow, new Dictionary<string, string>());
            //    trackingContext.SetAsCurrent();

            await m_Harness!.TestService(
                callerMock,
                registrationEngineMockService);//,
                //new Web.UseCases());
        }

    }
}