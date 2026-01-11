namespace Application.DTOs.Product;

public record UpdateProductRequest(
	string Name,
	decimal Price
);
