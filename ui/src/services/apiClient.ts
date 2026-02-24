import axios from 'axios';

const apiClient = axios.create({
    baseURL: '',
    headers: {
        'Content-Type': 'application/json',
    },
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response) {
            const { status, data } = error.response;
            console.error(`API Error [${status}]:`, data);
        } else if (error.request) {
            console.error('No response from server:', error.message);
        } else {
            console.error('Request error:', error.message);
        }
        return Promise.reject(error);
    }
);

export default apiClient;
