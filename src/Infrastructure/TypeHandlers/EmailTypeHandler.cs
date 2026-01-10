using System.Data;
using Dapper;
using Domain.ValueObjects;

namespace Infrastructure.TypeHandlers;

public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
	public override void SetValue(IDbDataParameter parameter, Email? value)
	{
		parameter.Value = value?.Value ?? (object)DBNull.Value;
	}

	public override Email? Parse(object value)
	{
		if (value is null || value == DBNull.Value)
			return null;

		return new Email(value.ToString()!);
	}
}
