using Application.DTOs.Customer;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Services;

public class CustomerServiceTests
{
	private readonly ICustomerRepository _customerRepository;
	private readonly CustomerService _sut;

	public CustomerServiceTests()
	{
		_customerRepository = Substitute.For<ICustomerRepository>();
		_sut = new CustomerService(_customerRepository);
	}

	[Fact]
	public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomerDto()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		_customerRepository.GetByIdAsync(1).Returns(customer);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("John Doe");
		result.Email.Should().Be("john@example.com");
	}

	[Fact]
	public async Task GetByIdAsync_WhenCustomerNotExists_ReturnsNull()
	{
		// Arrange
		_customerRepository.GetByIdAsync(1).Returns((Customer?)null);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetByEmailAsync_WhenCustomerExists_ReturnsCustomerDto()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		_customerRepository.GetByEmailAsync("john@example.com").Returns(customer);

		// Act
		var result = await _sut.GetByEmailAsync("john@example.com");

		// Assert
		result.Should().NotBeNull();
		result!.Email.Should().Be("john@example.com");
	}

	[Fact]
	public async Task GetAllAsync_ReturnsAllCustomers()
	{
		// Arrange
		var customers = new List<Customer>
		{
			new("John Doe", new Email("john@example.com")),
			new("Jane Doe", new Email("jane@example.com"))
		};
		_customerRepository.GetAllAsync().Returns(customers);

		// Act
		var result = await _sut.GetAllAsync();

		// Assert
		result.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetPagedAsync_ReturnsPagedResult()
	{
		// Arrange
		var customers = new List<Customer>
		{
			new("John Doe", new Email("john@example.com"))
		};
		var pagedResult = new PagedResult<Customer>(customers, 1, 10, 1);
		_customerRepository.GetPagedAsync(1, 10).Returns(pagedResult);

		// Act
		var result = await _sut.GetPagedAsync(1, 10);

		// Assert
		result.Items.Should().HaveCount(1);
		result.PageNumber.Should().Be(1);
		result.TotalCount.Should().Be(1);
	}

	[Fact]
	public async Task CreateAsync_ValidRequest_ReturnsNewId()
	{
		// Arrange
		var request = new CreateCustomerRequest("John Doe", "john@example.com");
		_customerRepository.AddAsync(Arg.Any<Customer>()).Returns(1);

		// Act
		var result = await _sut.CreateAsync(request);

		// Assert
		result.Should().Be(1);
		await _customerRepository.Received(1).AddAsync(Arg.Any<Customer>());
	}

	[Fact]
	public async Task UpdateAsync_WhenCustomerExists_ReturnsTrue()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		_customerRepository.GetByIdAsync(1).Returns(customer);
		_customerRepository.UpdateAsync(Arg.Any<Customer>()).Returns(true);

		var request = new UpdateCustomerRequest("Jane Doe", "jane@example.com");

		// Act
		var result = await _sut.UpdateAsync(1, request);

		// Assert
		result.Should().BeTrue();
		await _customerRepository.Received(1).UpdateAsync(Arg.Any<Customer>());
	}

	[Fact]
	public async Task UpdateAsync_WhenCustomerNotExists_ReturnsFalse()
	{
		// Arrange
		_customerRepository.GetByIdAsync(1).Returns((Customer?)null);
		var request = new UpdateCustomerRequest("Jane Doe", "jane@example.com");

		// Act
		var result = await _sut.UpdateAsync(1, request);

		// Assert
		result.Should().BeFalse();
		await _customerRepository.DidNotReceive().UpdateAsync(Arg.Any<Customer>());
	}

	[Fact]
	public async Task DeleteAsync_CallsRepository()
	{
		// Arrange
		_customerRepository.DeleteAsync(1).Returns(true);

		// Act
		var result = await _sut.DeleteAsync(1);

		// Assert
		result.Should().BeTrue();
		await _customerRepository.Received(1).DeleteAsync(1);
	}
}
