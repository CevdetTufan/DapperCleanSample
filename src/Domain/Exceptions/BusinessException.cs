namespace Domain.Exceptions;

/// <summary>
/// Base exception for all business/domain errors.
/// These exceptions are safe to expose to end users.
/// </summary>
public abstract class BusinessException : Exception
{
	/// <summary>
	/// A URI reference that identifies the problem type (RFC 9457).
	/// </summary>
	public abstract string Type { get; }

	/// <summary>
	/// A short, human-readable summary of the problem type.
	/// </summary>
	public abstract string Title { get; }

	/// <summary>
	/// The HTTP status code for this error.
	/// </summary>
	public abstract int StatusCode { get; }

	protected BusinessException(string message) : base(message) { }

	protected BusinessException(string message, Exception innerException)
		: base(message, innerException) { }
}
