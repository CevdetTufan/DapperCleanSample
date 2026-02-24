namespace Domain.Exceptions;

/// <summary>
/// Thrown when input validation fails.
/// </summary>
public sealed class ValidationException : BusinessException
{
	public override string Type => "https://tools.ietf.org/html/rfc9110#section-15.5.1";
	public override string Title => "Validation Error";
	public override int StatusCode => 400;

	public ValidationException(string message) : base(message)
	{
		Errors = new Dictionary<string, string[]>();
	}

	public ValidationException(string field, string message)
		: base(message)
	{
		Errors = new Dictionary<string, string[]>
		{
			[field] = [message]
		};
	}

	public ValidationException(IDictionary<string, string[]> errors)
		: base("One or more validation errors occurred.")
	{
		Errors = errors;
	}

	public IDictionary<string, string[]> Errors { get; }
}
