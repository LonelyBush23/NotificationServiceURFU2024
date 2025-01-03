using MediatR;

namespace ApiGateway.Application.Infrastructure.Cqs
{
    public class Command : IRequest<Result.Result>, ICommand
    {
    }
}
