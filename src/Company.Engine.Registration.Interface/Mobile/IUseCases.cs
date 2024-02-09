﻿using Company.Engine.Registration.Data.Mobile;
using ProtoBuf.Grpc;

namespace Company.Engine.Registration.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterMemberAsync(RegisterRequest request, CallContext context = default);

        Task<RegisterResponse> RegisterAccountAsync(RegisterRequest request, CallContext context = default);
    }
}