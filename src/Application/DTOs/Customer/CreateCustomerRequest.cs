namespace Application.DTOs.Customer;

public record CreateCustomerRequest(
	string Name,
	string Email
);