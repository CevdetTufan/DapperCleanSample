using Application.DTOs.Customer;
using Domain.Entities;

namespace Application.Mappings;

public static class CustomerMappingExtensions
{
	public static CustomerDto ToDto(this Customer customer)
	{
		return new CustomerDto(
			customer.Id,
			customer.Name,
			customer.Email.Value,
			customer.CreatedAt
		);
	}
}
