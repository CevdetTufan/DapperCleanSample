# Dapper Clean Architecture Sample

> **This project was created using Claude Opus 4.5**

A .NET 10 sample project demonstrating Clean Architecture principles with Dapper ORM. This project showcases best practices for building maintainable, testable, and scalable applications.

## ??? Architecture Overview

This project follows **Clean Architecture** principles with clear separation of concerns:

```
???????????????????????????????????????????????????????????????
?                        API Layer                            ?
?                    (Coming Soon)                            ?
???????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????
?                    Application Layer                        ?
?                    (Coming Soon)                            ?
???????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????
?                      Domain Layer                           ?
?         Entities, Value Objects, Interfaces                 ?
???????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????
?                  Infrastructure Layer                       ?
?            Dapper Repositories, Data Access                 ?
???????????????????????????????????????????????????????????????
```

## ?? Project Structure

```
DapperCleanSample/
??? src/
?   ??? Domain/                          # Core business logic
?   ?   ??? Common/
?   ?   ?   ??? PagedResult.cs          # Pagination support
?   ?   ??? Entities/
?   ?   ?   ??? Product.cs              # Product entity with validation
?   ?   ?   ??? Customer.cs             # Customer entity with Email VO
?   ?   ?   ??? Order.cs                # Order entity with state machine
?   ?   ?   ??? OrderItem.cs            # Order item entity
?   ?   ??? Enums/
?   ?   ?   ??? OrderStatus.cs          # Order status enumeration
?   ?   ??? Exceptions/
?   ?   ?   ??? DomainException.cs      # Base domain exception
?   ?   ?   ??? EntityNotFoundException.cs
?   ?   ??? Interfaces/
?   ?   ?   ??? IProductRepository.cs   # Product repository contract
?   ?   ?   ??? ICustomerRepository.cs  # Customer repository contract
?   ?   ?   ??? IOrderRepository.cs     # Order repository contract
?   ?   ?   ??? IOrderItemRepository.cs # OrderItem repository contract
?   ?   ??? ValueObjects/
?   ?       ??? Email.cs                # Email value object with validation
?   ?
?   ??? Infrastructure/                  # Data access implementation
?       ??? Data/
?       ?   ??? DapperContext.cs        # Database connection factory
?       ??? Repositories/
?       ?   ??? ProductRepository.cs    # Dapper product repository
?       ?   ??? CustomerRepository.cs   # Dapper customer repository
?       ?   ??? OrderRepository.cs      # Dapper order repository
?       ?   ??? OrderItemRepository.cs  # Dapper order item repository
?       ??? TypeHandlers/
?       ?   ??? EmailTypeHandler.cs     # Dapper type handler for Email VO
?       ??? DependencyInjection.cs      # DI configuration
?
??? tests/
    ??? Domain.UnitTest/                 # Domain layer unit tests
    ?   ??? Entities/
    ?   ?   ??? ProductTests.cs
    ?   ?   ??? CustomerTests.cs
    ?   ?   ??? OrderTests.cs
    ?   ?   ??? OrderItemTests.cs
    ?   ??? ValueObjects/
    ?       ??? EmailTests.cs
    ?
    ??? Infrastructure.IntegrationTests/ # Repository integration tests
        ??? Fixtures/
        ?   ??? DatabaseFixture.cs      # SQLite in-memory setup
        ?   ??? EmailTypeHandler.cs     # Test type handler
        ?   ??? TestDapperContext.cs
        ??? Repositories/
        ?   ??? ProductRepositoryTests.cs
        ?   ??? CustomerRepositoryTests.cs
        ?   ??? OrderRepositoryTests.cs
        ?   ??? Test*Repository.cs      # Test repository implementations
        ??? ModuleInitializer.cs        # Assembly initialization
```

## ??? Technologies & Libraries

### Core
| Package | Version | Purpose |
|---------|---------|---------|
| .NET | 10.0 | Target framework |
| Dapper | 2.1.66 | Micro ORM for data access |
| Microsoft.Data.SqlClient | 6.1.3 | SQL Server connectivity |

### Testing
| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.9.3 | Testing framework |
| FluentAssertions | 8.8.0 | Fluent assertion library |
| Microsoft.Data.Sqlite | 10.0.1 | In-memory database for tests |
| coverlet.collector | 6.0.4 | Code coverage |

## ?? Key Features

### Domain Layer

#### Entities with Encapsulation
All entities use `private set` for properties with validation in constructors and update methods:

```csharp
public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product() { } // For Dapper

    public Product(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
        CreatedAt = DateTime.UtcNow;
    }
}
```

#### Value Objects
Immutable value objects with built-in validation:

```csharp
public sealed record Email
{
    private static readonly Regex ValidationRegex = 
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");
        if (!ValidationRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format");
        Value = value;
    }
}
```

#### Order State Machine
Order entity with controlled state transitions:

```
Pending ? Paid ? Shipped ? Delivered
   ?        ?
Cancelled  Cancelled (only from Pending/Paid)
```

### Infrastructure Layer

#### Dapper Repositories
Specific repositories with parameterized queries (SQL injection safe):

```csharp
public async Task<Product?> GetByIdAsync(int id)
{
    const string sql = "SELECT * FROM Products WHERE Id = @Id";
    using var connection = _context.CreateConnection();
    return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
}
```

#### Pagination Support
Built-in pagination with `PagedResult<T>`:

```csharp
public async Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize)
{
    const string countSql = "SELECT COUNT(*) FROM Products";
    const string dataSql = """
        SELECT * FROM Products
        ORDER BY Id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY
        """;
    // ...
}
```

#### Custom Type Handlers
Dapper type handlers for Value Objects:

```csharp
public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
    public override Email? Parse(object value)
    {
        return new Email(value.ToString()!);
    }

    public override void SetValue(IDbDataParameter parameter, Email? value)
    {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
    }
}
```

## ?? Testing

### Run All Tests
```bash
dotnet test
```

### Run Unit Tests Only
```bash
dotnet test tests/Domain.UnitTest
```

### Run Integration Tests Only
```bash
dotnet test tests/Infrastructure.IntegrationTests
```

### Test Coverage

| Layer | Test Type | Tests |
|-------|-----------|-------|
| Domain | Unit Tests | 28+ |
| Infrastructure | Integration Tests | 19+ |

## ?? Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (for production)

### Installation

1. Clone the repository
```bash
git clone https://github.com/CevdetTufan/DapperCleanSample.git
```

2. Navigate to the project directory
```bash
cd DapperCleanSample
```

3. Restore packages
```bash
dotnet restore
```

4. Build the solution
```bash
dotnet build
```

5. Run tests
```bash
dotnet test
```

## ?? Design Decisions

### Why Dapper over EF Core?
- **Performance**: Dapper is faster for read-heavy operations
- **Control**: Full SQL control for complex queries
- **Simplicity**: Less abstraction, more predictable behavior

### Why Specific Repositories over Generic?
- **Flexibility**: Each repository can have custom methods
- **Dapper Philosophy**: Embraces SQL, not abstracts it
- **Type Safety**: Compile-time checking for repository methods

### Why Private Setters?
- **Encapsulation**: Protects entity invariants
- **Dapper Compatible**: Works via reflection
- **Domain Logic**: Business rules enforced in entity

## ?? License

This project is open source and available under the [MIT License](LICENSE).

## ?? Contributing

Contributions, issues, and feature requests are welcome!

---

**Created with Claude Opus 4.5** - AI-assisted development for Clean Architecture with Dapper
