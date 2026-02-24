import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Typography,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TablePagination,
    Button,
    IconButton,
    Skeleton,
    Tooltip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import VisibilityIcon from '@mui/icons-material/Visibility';
import DeleteIcon from '@mui/icons-material/Delete';
import { orderService } from '../../services/orderService';
import type { OrderDto } from '../../types';
import StatusBadge from '../../components/StatusBadge/StatusBadge';
import ConfirmDialog from '../../components/ConfirmDialog/ConfirmDialog';
import { useToast } from '../../components/Toast/ToastProvider';

const OrderList: React.FC = () => {
    const { showToast } = useToast();
    const navigate = useNavigate();
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(0);
    const [pageSize] = useState(10);
    const [totalCount, setTotalCount] = useState(0);
    const [deleteTarget, setDeleteTarget] = useState<OrderDto | null>(null);

    const fetchOrders = useCallback(async () => {
        setLoading(true);
        try {
            const result = await orderService.getPaged(page + 1, pageSize);
            setOrders(result.items);
            setTotalCount(result.totalCount);
        } catch (error) {
            showToast(error instanceof Error ? error.message : 'Failed to load orders', 'error');
        } finally {
            setLoading(false);
        }
    }, [page, pageSize, showToast]);

    useEffect(() => {
        fetchOrders();
    }, [fetchOrders]);

    const handleDelete = async () => {
        if (!deleteTarget) return;
        try {
            await orderService.delete(deleteTarget.id);
            showToast('Order deleted successfully');
            setDeleteTarget(null);
            fetchOrders();
        } catch (error) {
            showToast(error instanceof Error ? error.message : 'Failed to delete order', 'error');
        }
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4">Orders</Typography>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => navigate('/orders/new')}
                >
                    New Order
                </Button>
            </Box>

            <Paper>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>ID</TableCell>
                                <TableCell>Customer ID</TableCell>
                                <TableCell>Date</TableCell>
                                <TableCell>Status</TableCell>
                                <TableCell align="right">Actions</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {loading ? (
                                Array.from({ length: 5 }).map((_, i) => (
                                    <TableRow key={i}>
                                        {Array.from({ length: 5 }).map((_, j) => (
                                            <TableCell key={j}><Skeleton /></TableCell>
                                        ))}
                                    </TableRow>
                                ))
                            ) : orders.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        <Typography color="text.secondary" sx={{ py: 4 }}>
                                            No orders found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                orders.map((order) => (
                                    <TableRow key={order.id} hover>
                                        <TableCell>#{order.id}</TableCell>
                                        <TableCell>{order.customerId}</TableCell>
                                        <TableCell>{new Date(order.orderDate).toLocaleDateString()}</TableCell>
                                        <TableCell>
                                            <StatusBadge status={order.status} />
                                        </TableCell>
                                        <TableCell align="right">
                                            <Tooltip title="View Details">
                                                <IconButton
                                                    size="small"
                                                    color="primary"
                                                    onClick={() => navigate(`/orders/${order.id}`)}
                                                >
                                                    <VisibilityIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Delete">
                                                <IconButton
                                                    size="small"
                                                    color="error"
                                                    onClick={() => setDeleteTarget(order)}
                                                >
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
                <TablePagination
                    component="div"
                    count={totalCount}
                    page={page}
                    onPageChange={(_, newPage) => setPage(newPage)}
                    rowsPerPage={pageSize}
                    rowsPerPageOptions={[10]}
                />
            </Paper>

            <ConfirmDialog
                open={!!deleteTarget}
                title="Delete Order"
                message={`Are you sure you want to delete order #${deleteTarget?.id}? This action cannot be undone.`}
                onConfirm={handleDelete}
                onCancel={() => setDeleteTarget(null)}
            />
        </Box>
    );
};

export default OrderList;
