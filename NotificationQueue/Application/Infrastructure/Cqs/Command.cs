using MediatR;
using System.Windows.Input;

namespace NotificationQueue.Application.Infrastructure.Cqs
{
    public class Command : IRequest<Result.Result>, ICommand
    {
    }
}
