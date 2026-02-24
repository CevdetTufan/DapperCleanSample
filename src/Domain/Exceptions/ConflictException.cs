namespace Domain.Exceptions;

/// <summary>
/// Thrown when a unique constraint is violated (e.g., duplicate email).
/// </summary>
public sealed class ConflictException : BusinessException
{
	public override string Type => "https://tools.ietf.org/html/rfc9110#section-15.5.10";
	public override string Title => "Conflict";
	public override int StatusCode => 409;

	public ConflictException(string message) : base(message) { }

	public ConflictException(string resourceName, string field, object value)
		: base($"{resourceName} with {field} '{value}' already exists.")
	{
		ResourceName = resourceName;
		Field = field;
		Value = value;
	}

	public string? ResourceName { get; }
	public string? Field { get; }
	public object? Value { get; }
}
