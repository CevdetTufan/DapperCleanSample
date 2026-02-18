import apiClient from './apiClient';
import type {
    ProductDto,
    CreateProductRequest,
    UpdateProductRequest,
    PagedResult,
} from '../types';

export const productService = {
    getAll: () =>
        apiClient.get<ProductDto[]>('/api/products').then((r) => r.data),

    getPaged: (page: number, size: number) =>
        apiClient
            .get<PagedResult<ProductDto>>('/api/products/paged', {
                params: { pageNumber: page, pageSize: size },
            })
            .then((r) => r.data),

    getById: (id: number) =>
        apiClient.get<ProductDto>(`/api/products/${id}`).then((r) => r.data),

    create: (data: CreateProductRequest) =>
        apiClient.post<{ id: number }>('/api/products', data).then((r) => r.data),

    update: (id: number, data: UpdateProductRequest) =>
        apiClient.put(`/api/products/${id}`, data),

    delete: (id: number) => apiClient.delete(`/api/products/${id}`),
};
