using Company.iFX.Test;
using Company.Manager.Membership.Interface;
using FluentAssertions;
using Moq;

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
            var webUseCasesMock = new Mock<Interface.Web.IUseCases>();

            webUseCasesMock
                .Setup(x => x.RegisterMemberAsync(It.IsAny<Data.Web.RegisterRequest>()))
                .Returns(() =>
                {
                    var response = new Data.Web.RegisterResponse
                    {
                        Name = "test",
                        WebMessage = "test"
                    };
                    return Task.FromResult(response);
                });

            Func<IMembershipManager, Task> serviceRunner = ServiceRunner.Create<IMembershipManager>(async service =>
            {
                var response = await service.RegisterMemberAsync(
                    new Data.Web.RegisterRequest
                    {
                        Name = ""
                    },
                    default);
                response.Should().NotBeNull();
                response.Name.Should().Be("test");
            });

            //    var trackingContext = new TrackingContext(Guid.Parse("blah"), DateTime.UtcNow, new Dictionary<string, string>());
            //    trackingContext.SetAsCurrent();

            await m_Harness!.TestService(
                serviceRunner,
                webUseCasesMock.Object);
        }

    }
}