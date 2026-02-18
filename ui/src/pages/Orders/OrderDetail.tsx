import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    Box,
    Typography,
    Paper,
    Grid,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Button,
    Skeleton,
    Divider,
    Chip,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PaymentIcon from '@mui/icons-material/Payment';
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import { orderService } from '../../services/orderService';
import type { OrderDto } from '../../types';
import { OrderStatus } from '../../types';
import StatusBadge from '../../components/StatusBadge/StatusBadge';
import { useToast } from '../../components/Toast/ToastProvider';

const OrderDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { showToast } = useToast();
    const [order, setOrder] = useState<OrderDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [actionLoading, setActionLoading] = useState(false);

    const fetchOrder = useCallback(async () => {
        if (!id) return;
        setLoading(true);
        try {
            const data = await orderService.getWithItems(parseInt(id));
            setOrder(data);
        } catch {
            showToast('Failed to load order details', 'error');
        } finally {
            setLoading(false);
        }
    }, [id, showToast]);

    useEffect(() => {
        fetchOrder();
    }, [fetchOrder]);

    const handleAction = async (action: () => Promise<unknown>, successMsg: string) => {
        setActionLoading(true);
        try {
            await action();
            showToast(successMsg);
            fetchOrder();
        } catch {
            showToast('Action failed', 'error');
        } finally {
            setActionLoading(false);
        }
    };

    const formatCurrency = (value: number) =>
        new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);

    if (loading) {
        return (
            <Box>
                <Skeleton width={200} height={40} />
                <Skeleton width="100%" height={200} sx={{ mt: 2 }} />
            </Box>
        );
    }

    if (!order) {
        return (
            <Box sx={{ textAlign: 'center', py: 8 }}>
                <Typography variant="h5" color="text.secondary">
                    Order not found
                </Typography>
                <Button onClick={() => navigate('/orders')} sx={{ mt: 2 }}>
                    Back to Orders
                </Button>
            </Box>
        );
    }

    const totalAmount = order.items?.reduce((sum, item) => sum + item.totalPrice, 0) ?? 0;

    return (
        <Box>
            <Button
                startIcon={<ArrowBackIcon />}
                onClick={() => navigate('/orders')}
                sx={{ mb: 2 }}
            >
                Back to Orders
            </Button>

            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
                <Typography variant="h4">Order #{order.id}</Typography>
                <StatusBadge status={order.status} />
            </Box>

            <Grid container spacing={3}>
                {/* Order Info */}
                <Grid size={{ xs: 12, md: 4 }}>
                    <Paper sx={{ p: 3 }}>
                        <Typography variant="h6" sx={{ mb: 2 }}>
                            Order Information
                        </Typography>
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                <Typography color="text.secondary">Customer ID</Typography>
                                <Typography fontWeight={500}>{order.customerId}</Typography>
                            </Box>
                            <Divider />
                            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                <Typography color="text.secondary">Order Date</Typography>
                                <Typography fontWeight={500}>
                                    {new Date(order.orderDate).toLocaleDateString()}
                                </Typography>
                            </Box>
                            <Divider />
                            <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                <Typography color="text.secondary">Total</Typography>
                                <Typography fontWeight={700} color="primary.main" variant="h6">
                                    {formatCurrency(totalAmount)}
                                </Typography>
                            </Box>
                        </Box>
                    </Paper>
                </Grid>

                {/* Status Actions */}
                <Grid size={{ xs: 12, md: 8 }}>
                    <Paper sx={{ p: 3 }}>
                        <Typography variant="h6" sx={{ mb: 2 }}>
                            Status Actions
                        </Typography>

                        {/* Status workflow visualization */}
                        <Box sx={{ display: 'flex', gap: 1, mb: 3, flexWrap: 'wrap', alignItems: 'center' }}>
                            {[
                                { label: 'Pending', status: OrderStatus.Pending },
                                { label: 'Paid', status: OrderStatus.Paid },
                                { label: 'Shipped', status: OrderStatus.Shipped },
                                { label: 'Delivered', status: OrderStatus.Delivered },
                            ].map((step, index) => (
                                <React.Fragment key={step.label}>
                                    {index > 0 && (
                                        <Typography color="text.disabled" sx={{ mx: 0.5 }}>→</Typography>
                                    )}
                                    <Chip
                                        label={step.label}
                                        variant={order.status === step.status ? 'filled' : 'outlined'}
                                        color={order.status === step.status ? 'primary' : 'default'}
                                        size="small"
                                    />
                                </React.Fragment>
                            ))}
                        </Box>

                        <Box sx={{ display: 'flex', gap: 1.5, flexWrap: 'wrap' }}>
                            {order.status === OrderStatus.Pending && (
                                <>
                                    <Button
                                        variant="contained"
                                        color="info"
                                        startIcon={<PaymentIcon />}
                                        disabled={actionLoading}
                                        onClick={() =>
                                            handleAction(() => orderService.markAsPaid(order.id), 'Order marked as paid')
                                        }
                                    >
                                        Mark as Paid
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        color="error"
                                        startIcon={<CancelIcon />}
                                        disabled={actionLoading}
                                        onClick={() =>
                                            handleAction(() => orderService.cancel(order.id), 'Order cancelled')
                                        }
                                    >
                                        Cancel Order
                                    </Button>
                                </>
                            )}
                            {order.status === OrderStatus.Paid && (
                                <>
                                    <Button
                                        variant="contained"
                                        color="secondary"
                                        startIcon={<LocalShippingIcon />}
                                        disabled={actionLoading}
                                        onClick={() =>
                                            handleAction(() => orderService.ship(order.id), 'Order shipped')
                                        }
                                    >
                                        Ship Order
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        color="error"
                                        startIcon={<CancelIcon />}
                                        disabled={actionLoading}
                                        onClick={() =>
                                            handleAction(() => orderService.cancel(order.id), 'Order cancelled')
                                        }
                                    >
                                        Cancel Order
                                    </Button>
                                </>
                            )}
                            {order.status === OrderStatus.Shipped && (
                                <Button
                                    variant="contained"
                                    color="success"
                                    startIcon={<CheckCircleIcon />}
                                    disabled={actionLoading}
                                    onClick={() =>
                                        handleAction(() => orderService.deliver(order.id), 'Order delivered')
                                    }
                                >
                                    Mark as Delivered
                                </Button>
                            )}
                            {order.status === OrderStatus.Delivered && (
                                <Typography color="success.main" sx={{ fontWeight: 600 }}>
                                    ✓ This order has been completed
                                </Typography>
                            )}
                            {order.status === OrderStatus.Cancelled && (
                                <Typography color="error.main" sx={{ fontWeight: 600 }}>
                                    ✕ This order has been cancelled
                                </Typography>
                            )}
                        </Box>
                    </Paper>
                </Grid>

                {/* Order Items */}
                <Grid size={12}>
                    <Paper sx={{ p: 3 }}>
                        <Typography variant="h6" sx={{ mb: 2 }}>
                            Order Items
                        </Typography>
                        <TableContainer>
                            <Table>
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Product ID</TableCell>
                                        <TableCell align="right">Quantity</TableCell>
                                        <TableCell align="right">Unit Price</TableCell>
                                        <TableCell align="right">Total</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {order.items && order.items.length > 0 ? (
                                        <>
                                            {order.items.map((item) => (
                                                <TableRow key={item.id}>
                                                    <TableCell>{item.productId}</TableCell>
                                                    <TableCell align="right">{item.quantity}</TableCell>
                                                    <TableCell align="right">{formatCurrency(item.unitPrice)}</TableCell>
                                                    <TableCell align="right" sx={{ fontWeight: 500 }}>
                                                        {formatCurrency(item.totalPrice)}
                                                    </TableCell>
                                                </TableRow>
                                            ))}
                                            <TableRow>
                                                <TableCell colSpan={3} align="right" sx={{ fontWeight: 700, borderBottom: 'none' }}>
                                                    Total:
                                                </TableCell>
                                                <TableCell align="right" sx={{ fontWeight: 700, color: 'primary.main', borderBottom: 'none' }}>
                                                    {formatCurrency(totalAmount)}
                                                </TableCell>
                                            </TableRow>
                                        </>
                                    ) : (
                                        <TableRow>
                                            <TableCell colSpan={4} align="center">
                                                <Typography color="text.secondary" sx={{ py: 2 }}>
                                                    No items
                                                </Typography>
                                            </TableCell>
                                        </TableRow>
                                    )}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Paper>
                </Grid>
            </Grid>
        </Box>
    );
};

export default OrderDetail;
