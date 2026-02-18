import apiClient from './apiClient';
import type {
    CustomerDto,
    CreateCustomerRequest,
    UpdateCustomerRequest,
    PagedResult,
} from '../types';

export const customerService = {
    getAll: () =>
        apiClient.get<CustomerDto[]>('/api/customers').then((r) => r.data),

    getPaged: (page: number, size: number) =>
        apiClient
            .get<PagedResult<CustomerDto>>('/api/customers/paged', {
                params: { pageNumber: page, pageSize: size },
            })
            .then((r) => r.data),

    getById: (id: number) =>
        apiClient.get<CustomerDto>(`/api/customers/${id}`).then((r) => r.data),

    getByEmail: (email: string) =>
        apiClient
            .get<CustomerDto>(`/api/customers/email/${email}`)
            .then((r) => r.data),

    create: (data: CreateCustomerRequest) =>
        apiClient
            .post<{ id: number }>('/api/customers', data)
            .then((r) => r.data),

    update: (id: number, data: UpdateCustomerRequest) =>
        apiClient.put(`/api/customers/${id}`, data),

    delete: (id: number) => apiClient.delete(`/api/customers/${id}`),
};
