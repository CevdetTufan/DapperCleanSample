import React from 'react';
import { Chip } from '@mui/material';
import { OrderStatus, OrderStatusLabels, OrderStatusColors } from '../../types';

interface StatusBadgeProps {
    status: OrderStatus;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
    return (
        <Chip
            label={OrderStatusLabels[status]}
            color={OrderStatusColors[status] as 'warning' | 'info' | 'secondary' | 'success' | 'error'}
            size="small"
            sx={{ fontWeight: 600 }}
        />
    );
};

export default StatusBadge;
