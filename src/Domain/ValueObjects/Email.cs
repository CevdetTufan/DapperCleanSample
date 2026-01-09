using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed record Email
{
	private static readonly Regex ValidationRegex =
		new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

	public string Value { get; }

	public Email(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Email cannot be empty", nameof(value));

		if (!ValidationRegex.IsMatch(value))
			throw new ArgumentException("Invalid email format", nameof(value));

		Value = value;
	}

	public override string ToString() => Value;

	public static implicit operator string(Email email) => email.Value;
}
