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
        // RFC 9457 Problem Details formatını parse et
        const data = error.response?.data;
        let message = 'An unexpected error occurred';

        if (data) {
            if (data.errors) {
                // Validation hataları: { "Field": ["msg1", "msg2"] }
                message = Object.values(data.errors as Record<string, string[]>)
                    .flat()
                    .join(', ');
            } else if (data.detail) {
                // Tek satır hata mesajı
                message = data.detail;
            } else if (data.title) {
                // Genel başlık (ör. "Bad Request")
                message = data.title;
            }
        } else if (error.message) {
            // Sunucuya ulaşılamadı vb.
            message = error.message;
        }

        return Promise.reject(new Error(message));
    }
);

export default apiClient;
