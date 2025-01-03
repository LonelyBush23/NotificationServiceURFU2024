namespace ApiGateway.Application.Infrastructure.Result;

public interface IResult<out T> : IResult
{
    T Value { get; }
}