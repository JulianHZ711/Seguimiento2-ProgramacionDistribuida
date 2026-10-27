using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Utilities.Mediator
{
    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface IRequestHandler<TRequest> where TRequest : IRequest
    {
        Task Handle(TRequest request);
    }
}
