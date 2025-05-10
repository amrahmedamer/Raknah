using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Raknah.Abstractions;

public static class ResultExtension
{
    public static ObjectResult ToProblem(this Result result)
    {

        if (result.IsSuccess)
            throw new InvalidOperationException("cannot convert success result to a problem");


        var problemDetails = new ProblemDetails()
        {
            Type = $"https://tools.ietf.org/html/rfc7231#section-6.{result.Error.StatusCode / 100}.{result.Error.StatusCode % 100}",
            Title = ReasonPhrases.GetReasonPhrase(result.Error.StatusCode),
            Status = result.Error.StatusCode,
            Extensions = new Dictionary<string, Object?>()
            {
                {"error",result.Error},
                {"traceId",Guid.NewGuid().ToString()}
            }
        };

        return new ObjectResult(problemDetails);
    }
}
