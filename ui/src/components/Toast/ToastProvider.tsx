import React, { createContext, useContext, useState, useCallback } from 'react';
import { Snackbar, Alert, type AlertColor } from '@mui/material';

interface ToastMessage {
    id: number;
    message: string;
    severity: AlertColor;
}

interface ToastContextType {
    showToast: (message: string, severity?: AlertColor) => void;
}

const ToastContext = createContext<ToastContextType>({
    showToast: () => { },
});

export const useToast = () => useContext(ToastContext);

let nextId = 0;

export const ToastProvider: React.FC<{ children: React.ReactNode }> = ({
    children,
}) => {
    const [toasts, setToasts] = useState<ToastMessage[]>([]);

    const showToast = useCallback(
        (message: string, severity: AlertColor = 'success') => {
            const id = nextId++;
            setToasts((prev) => [...prev, { id, message, severity }]);
        },
        []
    );

    const handleClose = (id: number) => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
    };

    return (
        <ToastContext.Provider value={{ showToast }}>
            {children}
            {toasts.map((toast, index) => (
                <Snackbar
                    key={toast.id}
                    open
                    autoHideDuration={4000}
                    onClose={() => handleClose(toast.id)}
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                    sx={{ bottom: `${(index * 60) + 24}px !important` }}
                >
                    <Alert
                        onClose={() => handleClose(toast.id)}
                        severity={toast.severity}
                        variant="filled"
                        sx={{ width: '100%' }}
                    >
                        {toast.message}
                    </Alert>
                </Snackbar>
            ))}
        </ToastContext.Provider>
    );
};
