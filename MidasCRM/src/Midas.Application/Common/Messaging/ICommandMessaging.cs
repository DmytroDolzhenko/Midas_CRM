using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Messaging
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }

    public interface ICommand : IRequest { }
}
