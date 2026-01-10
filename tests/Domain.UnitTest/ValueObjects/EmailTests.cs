using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.UnitTests.ValueObjects;

public class EmailTests
{
	[Theory]
	[InlineData("test@example.com")]
	[InlineData("user.name@domain.org")]
	[InlineData("user+tag@domain.co.uk")]
	public void Constructor_WithValidEmail_ShouldCreateEmail(string validEmail)
	{
		// Act
		var email = new Email(validEmail);

		// Assert
		email.Value.Should().Be(validEmail);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithEmptyEmail_ShouldThrowArgumentException(string? invalidEmail)
	{
		// Act
		var act = () => new Email(invalidEmail!);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Email cannot be empty*");
	}

	[Theory]
	[InlineData("invalid")]
	[InlineData("invalid@")]
	[InlineData("@domain.com")]
	[InlineData("invalid@domain")]
	public void Constructor_WithInvalidFormat_ShouldThrowArgumentException(string invalidEmail)
	{
		// Act
		var act = () => new Email(invalidEmail);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Invalid email format*");
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		// Arrange
		var email = new Email("test@example.com");

		// Act
		string result = email;

		// Assert
		result.Should().Be("test@example.com");
	}

	[Fact]
	public void ToString_ShouldReturnValue()
	{
		// Arrange
		var email = new Email("test@example.com");

		// Act
		var result = email.ToString();

		// Assert
		result.Should().Be("test@example.com");
	}
}