namespace ApiGateway.Application.Infrastructure.Result;

public interface IError
{
    string Type { get; }
    Dictionary<string, object> Data { get; }
}