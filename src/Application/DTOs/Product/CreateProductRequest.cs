namespace Application.DTOs.Product;

public record CreateProductRequest(
	string Name,
	decimal Price
);
