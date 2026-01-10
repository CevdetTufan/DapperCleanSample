using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class CustomerTests
{
	[Fact]
	public void Constructor_WithValidParameters_ShouldCreateCustomer()
	{
		// Arrange
		var email = new Email("test@example.com");

		// Act
		var customer = new Customer("John Doe", email);

		// Assert
		customer.Name.Should().Be("John Doe");
		customer.Email.Value.Should().Be("test@example.com");
		customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
	{
		// Arrange
		var email = new Email("test@example.com");

		// Act
		var act = () => new Customer(invalidName!, email);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Name cannot be empty*");
	}

	[Fact]
	public void UpdateName_WithValidName_ShouldUpdateName()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("test@example.com"));

		// Act
		customer.UpdateName("Jane Doe");

		// Assert
		customer.Name.Should().Be("Jane Doe");
	}

	[Fact]
	public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("test@example.com"));
		var newEmail = new Email("newemail@example.com");

		// Act
		customer.UpdateEmail(newEmail);

		// Assert
		customer.Email.Value.Should().Be("newemail@example.com");
	}
}