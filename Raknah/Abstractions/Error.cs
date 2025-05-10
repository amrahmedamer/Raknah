namespace Raknah.Abstractions;

public record Error(string Code, string Description, int StatusCode)
{
    public static Error None = new Error(string.Empty, string.Empty, 0);
}
