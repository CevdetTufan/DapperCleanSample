namespace Application.DTOs.Customer;

public record UpdateCustomerRequest(
	string Name,
	string Email
);