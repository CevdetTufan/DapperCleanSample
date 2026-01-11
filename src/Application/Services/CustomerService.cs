using Application.DTOs.Customer;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;

namespace Application.Services;

public class CustomerService : ICustomerService
{
	private readonly ICustomerRepository _customerRepository;

	public CustomerService(ICustomerRepository customerRepository)
	{
		_customerRepository = customerRepository;
	}

	public async Task<CustomerDto?> GetByIdAsync(int id)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		return customer is null ? null : MapToDto(customer);
	}

	public async Task<CustomerDto?> GetByEmailAsync(string email)
	{
		var customer = await _customerRepository.GetByEmailAsync(email);
		return customer is null ? null : MapToDto(customer);
	}

	public async Task<IEnumerable<CustomerDto>> GetAllAsync()
	{
		var customers = await _customerRepository.GetAllAsync();
		return customers.Select(MapToDto);
	}

	public async Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize)
	{
		var result = await _customerRepository.GetPagedAsync(pageNumber, pageSize);
		var dtos = result.Items.Select(MapToDto);
		return new PagedResult<CustomerDto>(dtos, pageNumber, pageSize, result.TotalCount);
	}

	public async Task<int> CreateAsync(CreateCustomerRequest request)
	{
		var email = new Email(request.Email);
		var customer = new Customer(request.Name, email);
		return await _customerRepository.AddAsync(customer);
	}

	public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest request)
	{
		var customer = await _customerRepository.GetByIdAsync(id);
		if (customer is null)
			return false;

		customer.UpdateName(request.Name);
		customer.UpdateEmail(new Email(request.Email));
		return await _customerRepository.UpdateAsync(customer);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		return await _customerRepository.DeleteAsync(id);
	}

	private static CustomerDto MapToDto(Customer customer)
	{
		return new CustomerDto(
			customer.Id,
			customer.Name,
			customer.Email.Value,
			customer.CreatedAt
		);
	}
}