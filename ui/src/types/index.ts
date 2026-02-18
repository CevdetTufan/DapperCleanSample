// Customer
export interface CustomerDto {
    id: number;
    name: string;
    email: string;
    createdAt: string;
}

export interface CreateCustomerRequest {
    name: string;
    email: string;
}

export interface UpdateCustomerRequest {
    name: string;
    email: string;
}

// Product
export interface ProductDto {
    id: number;
    name: string;
    price: number;
    createdAt: string;
}

export interface CreateProductRequest {
    name: string;
    price: number;
}

export interface UpdateProductRequest {
    name: string;
    price: number;
}

// Order
export interface OrderDto {
    id: number;
    customerId: number;
    orderDate: string;
    status: OrderStatus;
    createdAt: string;
    items?: OrderItemDto[];
}

export interface OrderItemDto {
    id: number;
    orderId: number;
    productId: number;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
}

export interface CreateOrderRequest {
    customerId: number;
    items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
    productId: number;
    quantity: number;
    unitPrice: number;
}

// Enums (using const object for compatibility with erasableSyntaxOnly)
export const OrderStatus = {
    Pending: 0,
    Paid: 1,
    Shipped: 2,
    Delivered: 3,
    Cancelled: 4,
} as const;

export type OrderStatus = (typeof OrderStatus)[keyof typeof OrderStatus];

export const OrderStatusLabels: Record<OrderStatus, string> = {
    [OrderStatus.Pending]: 'Pending',
    [OrderStatus.Paid]: 'Paid',
    [OrderStatus.Shipped]: 'Shipped',
    [OrderStatus.Delivered]: 'Delivered',
    [OrderStatus.Cancelled]: 'Cancelled',
};

export const OrderStatusColors: Record<OrderStatus, string> = {
    [OrderStatus.Pending]: 'warning',
    [OrderStatus.Paid]: 'info',
    [OrderStatus.Shipped]: 'secondary',
    [OrderStatus.Delivered]: 'success',
    [OrderStatus.Cancelled]: 'error',
};

// Pagination
export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}
