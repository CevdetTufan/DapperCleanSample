namespace Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
	public string EntityName { get; }
	public object EntityId { get; }

	public EntityNotFoundException(string entityName, object id)
		: base($"{entityName} with Id '{id}' was not found.")
	{
		EntityName = entityName;
		EntityId = id;
	}
}