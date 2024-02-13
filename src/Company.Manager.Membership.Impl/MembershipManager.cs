﻿using Company.iFX.Proxy;
using Company.Manager.Membership.Data;
using Company.Manager.Membership.Interface;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Manager.Membership.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class MembershipManager
        : IMembershipManager
    {
        private readonly ILogger m_Logger;

        public MembershipManager()
        {
            m_Logger = Proxy.CreateLogger<IMembershipManager>();
        }

        public async Task<RegisterResponseBase> RegisterMemberAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterMemberAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterMemberAsync)} {registerRequest}");

            return await UseCaseFactory<IMembershipManager, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }

        public async Task<RegisterResponseBase> RegisterAccountAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterAccountAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAccountAsync)} {registerRequest}");

            return await UseCaseFactory<IMembershipManager, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }
    }
}
