namespace Application.DTOs.Customer;

public record CustomerDto(
	int Id,
	string Name,
	string Email,
	DateTime CreatedAt
);