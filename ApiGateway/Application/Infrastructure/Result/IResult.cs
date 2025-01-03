namespace ApiGateway.Application.Infrastructure.Result;

public interface IResult
{
    bool IsSuccessfull { get; }
    IReadOnlyList<IError> GetErrors();
}