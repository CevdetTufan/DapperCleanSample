namespace Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource is not found.
/// </summary>
public sealed class NotFoundException : BusinessException
{
	public override string Type => "https://tools.ietf.org/html/rfc9110#section-15.5.5";
	public override string Title => "Resource Not Found";
	public override int StatusCode => 404;

	public NotFoundException(string resourceName, object key)
		: base($"{resourceName} with key '{key}' was not found.")
	{
		ResourceName = resourceName;
		Key = key;
	}

	public string ResourceName { get; }
	public object Key { get; }
}
