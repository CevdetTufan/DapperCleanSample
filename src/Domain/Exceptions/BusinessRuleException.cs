namespace Domain.Exceptions;

/// <summary>
/// Thrown when a business rule is violated.
/// </summary>
public sealed class BusinessRuleException : BusinessException
{
	public override string Type => "https://tools.ietf.org/html/rfc9110#section-15.5.10";
	public override string Title => "Business Rule Violation";
	public override int StatusCode => 409; // Conflict

	public BusinessRuleException(string message) : base(message) { }

	public BusinessRuleException(string rule, string message)
		: base(message)
	{
		Rule = rule;
	}

	public string? Rule { get; }
}
