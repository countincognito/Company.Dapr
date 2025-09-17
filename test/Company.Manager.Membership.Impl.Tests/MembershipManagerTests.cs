using Company.iFX.Test;
using Company.Manager.Membership.Interface;
using Moq;
using ProtoBuf.Grpc;
using Shouldly;

namespace Company.Manager.Membership.Impl.Tests
{
    public class MembershipManagerTests
        : IDisposable
    {
        private readonly UnitTestEnvironment m_TestEnvironment;

        public MembershipManagerTests()
        {
            m_TestEnvironment = new UnitTestEnvironment();
            m_TestEnvironment.Setup(typeof(MembershipManager));
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
                if (m_TestEnvironment is not null)
                {
                    m_TestEnvironment.Cleanup();
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
                .Setup(x => x.RegisterMemberAsync(It.IsAny<Data.Web.RegisterRequest>(), It.IsAny<CallContext>()))
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
                        Name = ServiceRunner.GenerateRandomString(),
                        Email = ServiceRunner.GenerateRandomString(),
                        DateOfBirth = ServiceRunner.GenerateRandomDateTime(),
                    },
                    default);
                response.ShouldNotBeNull();
                var webResponse = response as Data.Web.RegisterResponse;
                webResponse.ShouldNotBeNull();
                webResponse!.Name.ShouldBe(name);
                webResponse!.WebMessage.ShouldBe(webMessage);
            });

            await m_TestEnvironment.TestService(
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
                .Setup(x => x.RegisterMemberAsync(It.IsAny<Data.Mobile.RegisterRequest>(), It.IsAny<CallContext>()))
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
                        Name = ServiceRunner.GenerateRandomString(),
                        Email = ServiceRunner.GenerateRandomString(),
                        Password = ServiceRunner.GenerateRandomString()
                    },
                    default);
                response.ShouldNotBeNull();
                var mobileResponse = response as Data.Mobile.RegisterResponse;
                mobileResponse.ShouldNotBeNull();
                mobileResponse!.Name.ShouldBe(name);
                mobileResponse!.MobileMessage.ShouldBe(mobileMessage);
            });

            await m_TestEnvironment.TestService(
                serviceRunner,
                mobileUseCasesMock.Object);
        }
    }
}