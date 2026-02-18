import React, { useEffect, useState } from 'react';
import {
    Box,
    Grid,
    Paper,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Skeleton,
} from '@mui/material';
import PeopleIcon from '@mui/icons-material/People';
import InventoryIcon from '@mui/icons-material/Inventory2';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import { customerService } from '../../services/customerService';
import { productService } from '../../services/productService';
import { orderService } from '../../services/orderService';
import type { OrderDto } from '../../types';
import StatusBadge from '../../components/StatusBadge/StatusBadge';

interface StatCard {
    title: string;
    value: number | null;
    icon: React.ReactNode;
    color: string;
    bgColor: string;
}

const Dashboard: React.FC = () => {
    const [customerCount, setCustomerCount] = useState<number | null>(null);
    const [productCount, setProductCount] = useState<number | null>(null);
    const [orderCount, setOrderCount] = useState<number | null>(null);
    const [recentOrders, setRecentOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [customers, products, orders] = await Promise.all([
                    customerService.getAll(),
                    productService.getAll(),
                    orderService.getPaged(1, 5),
                ]);
                setCustomerCount(customers.length);
                setProductCount(products.length);
                setOrderCount(orders.totalCount);
                setRecentOrders(orders.items);
            } catch (error) {
                console.error('Failed to load dashboard data:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    const stats: StatCard[] = [
        {
            title: 'Total Customers',
            value: customerCount,
            icon: <PeopleIcon sx={{ fontSize: 32 }} />,
            color: '#6366f1',
            bgColor: '#eef2ff',
        },
        {
            title: 'Total Products',
            value: productCount,
            icon: <InventoryIcon sx={{ fontSize: 32 }} />,
            color: '#8b5cf6',
            bgColor: '#f5f3ff',
        },
        {
            title: 'Total Orders',
            value: orderCount,
            icon: <ShoppingCartIcon sx={{ fontSize: 32 }} />,
            color: '#06b6d4',
            bgColor: '#ecfeff',
        },
        {
            title: 'Revenue',
            value: null,
            icon: <TrendingUpIcon sx={{ fontSize: 32 }} />,
            color: '#10b981',
            bgColor: '#ecfdf5',
        },
    ];

    return (
        <Box>
            <Typography variant="h4" sx={{ mb: 3 }}>
                Dashboard
            </Typography>

            {/* Stat Cards */}
            <Grid container spacing={3} sx={{ mb: 4 }}>
                {stats.map((stat) => (
                    <Grid size={{ xs: 12, sm: 6, md: 3 }} key={stat.title}>
                        <Paper
                            sx={{
                                p: 3,
                                display: 'flex',
                                alignItems: 'center',
                                gap: 2,
                                transition: 'transform 0.2s, box-shadow 0.2s',
                                '&:hover': {
                                    transform: 'translateY(-2px)',
                                    boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
                                },
                            }}
                        >
                            <Box
                                sx={{
                                    p: 1.5,
                                    borderRadius: 3,
                                    bgcolor: stat.bgColor,
                                    color: stat.color,
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                }}
                            >
                                {stat.icon}
                            </Box>
                            <Box>
                                <Typography variant="body2" color="text.secondary" sx={{ fontWeight: 500 }}>
                                    {stat.title}
                                </Typography>
                                {loading ? (
                                    <Skeleton width={60} height={36} />
                                ) : (
                                    <Typography variant="h5">
                                        {stat.value !== null ? stat.value : '—'}
                                    </Typography>
                                )}
                            </Box>
                        </Paper>
                    </Grid>
                ))}
            </Grid>

            {/* Recent Orders */}
            <Paper sx={{ p: 3 }}>
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Recent Orders
                </Typography>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>ID</TableCell>
                                <TableCell>Customer ID</TableCell>
                                <TableCell>Date</TableCell>
                                <TableCell>Status</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {loading ? (
                                Array.from({ length: 5 }).map((_, i) => (
                                    <TableRow key={i}>
                                        {Array.from({ length: 4 }).map((_, j) => (
                                            <TableCell key={j}>
                                                <Skeleton />
                                            </TableCell>
                                        ))}
                                    </TableRow>
                                ))
                            ) : recentOrders.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={4} align="center">
                                        <Typography color="text.secondary" sx={{ py: 4 }}>
                                            No orders yet
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                recentOrders.map((order) => (
                                    <TableRow
                                        key={order.id}
                                        hover
                                        sx={{ cursor: 'pointer' }}
                                    >
                                        <TableCell>#{order.id}</TableCell>
                                        <TableCell>{order.customerId}</TableCell>
                                        <TableCell>
                                            {new Date(order.orderDate).toLocaleDateString()}
                                        </TableCell>
                                        <TableCell>
                                            <StatusBadge status={order.status} />
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Paper>
        </Box>
    );
};

export default Dashboard;
