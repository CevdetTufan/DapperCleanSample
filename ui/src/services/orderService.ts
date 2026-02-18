import apiClient from './apiClient';
import type {
    OrderDto,
    CreateOrderRequest,
    PagedResult,
} from '../types';

export const orderService = {
    getPaged: (page: number, size: number) =>
        apiClient
            .get<PagedResult<OrderDto>>('/api/orders/paged', {
                params: { pageNumber: page, pageSize: size },
            })
            .then((r) => r.data),

    getById: (id: number) =>
        apiClient.get<OrderDto>(`/api/orders/${id}`).then((r) => r.data),

    getWithItems: (id: number) =>
        apiClient.get<OrderDto>(`/api/orders/${id}/details`).then((r) => r.data),

    getByCustomer: (customerId: number) =>
        apiClient
            .get<OrderDto[]>(`/api/orders/customer/${customerId}`)
            .then((r) => r.data),

    create: (data: CreateOrderRequest) =>
        apiClient.post<{ id: number }>('/api/orders', data).then((r) => r.data),

    markAsPaid: (id: number) =>
        apiClient.patch(`/api/orders/${id}/pay`),

    ship: (id: number) =>
        apiClient.patch(`/api/orders/${id}/ship`),

    deliver: (id: number) =>
        apiClient.patch(`/api/orders/${id}/deliver`),

    cancel: (id: number) =>
        apiClient.patch(`/api/orders/${id}/cancel`),

    delete: (id: number) =>
        apiClient.delete(`/api/orders/${id}`),
};
