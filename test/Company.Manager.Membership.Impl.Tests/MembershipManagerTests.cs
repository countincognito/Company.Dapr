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
        public async Task MembershipManager_GivenRegisterMember_WhenWebRequest_ThenWebResponseReturned()
        {
            var webUseCasesMock = new Mock<Interface.Web.IUseCases>();
            string name = ServiceRunner.GenerateRandomString();
            string webMessage = ServiceRunner.GenerateRandomString();

            webUseCasesMock
                .Setup(x => x.RegisterMemberAsync(It.IsAny<Data.Web.RegisterRequest>()))
                .Returns(() =>
                {
                    var response = new Data.Web.RegisterResponse
                    {
                        Name = name,
                        WebMessage = webMessage
                    };
                    return Task.FromResult(response);
                });

            Func<IMembershipManager, Task> serviceRunner = ServiceRunner.Create<IMembershipManager>(async service =>
            {
                var response = await service.RegisterMemberAsync(
                    new Data.Web.RegisterRequest
                    {
                        Name = ServiceRunner.GenerateRandomString()
                    },
                    default);
                response.Should().NotBeNull();
                var webResponse = response as Data.Web.RegisterResponse;
                webResponse.Should().NotBeNull();
                webResponse!.Name.Should().Be(name);
                webResponse!.WebMessage.Should().Be(webMessage);
            });

            await m_Harness!.TestService(
                serviceRunner,
                webUseCasesMock.Object);
        }

        [Fact]
        public async Task MembershipManager_GivenRegisterMember_WhenMobileRequest_ThenMobileResponseReturned()
        {
            var mobileUseCasesMock = new Mock<Interface.Mobile.IUseCases>();
            string name = ServiceRunner.GenerateRandomString();
            string mobileMessage = ServiceRunner.GenerateRandomString();

            mobileUseCasesMock
                .Setup(x => x.RegisterMemberAsync(It.IsAny<Data.Mobile.RegisterRequest>()))
                .Returns(() =>
                {
                    var response = new Data.Mobile.RegisterResponse
                    {
                        Name = name,
                        MobileMessage = mobileMessage
                    };
                    return Task.FromResult(response);
                });

            Func<IMembershipManager, Task> serviceRunner = ServiceRunner.Create<IMembershipManager>(async service =>
            {
                var response = await service.RegisterMemberAsync(
                    new Data.Mobile.RegisterRequest
                    {
                        Name = ServiceRunner.GenerateRandomString()
                    },
                    default);
                response.Should().NotBeNull();
                var mobileResponse = response as Data.Mobile.RegisterResponse;
                mobileResponse.Should().NotBeNull();
                mobileResponse!.Name.Should().Be(name);
                mobileResponse!.MobileMessage.Should().Be(mobileMessage);
            });

            await m_Harness!.TestService(
                serviceRunner,
                mobileUseCasesMock.Object);
        }
    }
}