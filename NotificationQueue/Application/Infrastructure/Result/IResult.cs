namespace NotificationQueue.Application.Result;

public interface IResult
{
    bool IsSuccessfull { get; }
    IReadOnlyList<IError> GetErrors();
}