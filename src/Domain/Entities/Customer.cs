using Domain.ValueObjects;

namespace Domain.Entities;

public class Customer
{
	public int Id { get; private set; }
	public string Name { get; private set; } = null!;
	public Email Email { get; private set; } = null!;
	public DateTime CreatedAt { get; private set; }

	private Customer() { }

	public Customer(string name, Email email)
	{
		SetName(name);
		Email = email;
		CreatedAt = DateTime.UtcNow;
	}

	public void UpdateName(string name) => SetName(name);

	public void UpdateEmail(Email email) => Email = email;

	private void SetName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty", nameof(name));

		if (name.Length > 100)
			throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));

		Name = name;
	}
}