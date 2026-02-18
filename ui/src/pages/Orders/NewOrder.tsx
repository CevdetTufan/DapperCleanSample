import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Typography,
    Paper,
    Button,
    TextField,
    IconButton,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    MenuItem,
    Select,
    FormControl,
    InputLabel,
    Divider,
    type SelectChangeEvent,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { customerService } from '../../services/customerService';
import { productService } from '../../services/productService';
import { orderService } from '../../services/orderService';
import type { CustomerDto, ProductDto, CreateOrderItemRequest } from '../../types';
import { useToast } from '../../components/Toast/ToastProvider';

interface OrderItemRow {
    productId: number | '';
    quantity: number;
    unitPrice: number;
}

const NewOrder: React.FC = () => {
    const navigate = useNavigate();
    const { showToast } = useToast();

    const [customers, setCustomers] = useState<CustomerDto[]>([]);
    const [products, setProducts] = useState<ProductDto[]>([]);
    const [selectedCustomerId, setSelectedCustomerId] = useState<number | ''>('');
    const [items, setItems] = useState<OrderItemRow[]>([
        { productId: '', quantity: 1, unitPrice: 0 },
    ]);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [customerList, productList] = await Promise.all([
                    customerService.getAll(),
                    productService.getAll(),
                ]);
                setCustomers(customerList);
                setProducts(productList);
            } catch {
                showToast('Failed to load data', 'error');
            }
        };
        fetchData();
    }, [showToast]);

    const handleProductChange = (index: number, productId: number) => {
        const product = products.find((p) => p.id === productId);
        const newItems = [...items];
        newItems[index] = {
            ...newItems[index],
            productId,
            unitPrice: product?.price ?? 0,
        };
        setItems(newItems);
    };

    const handleQuantityChange = (index: number, quantity: number) => {
        const newItems = [...items];
        newItems[index] = { ...newItems[index], quantity: Math.max(1, quantity) };
        setItems(newItems);
    };

    const handlePriceChange = (index: number, price: number) => {
        const newItems = [...items];
        newItems[index] = { ...newItems[index], unitPrice: price };
        setItems(newItems);
    };

    const addItem = () => {
        setItems([...items, { productId: '', quantity: 1, unitPrice: 0 }]);
    };

    const removeItem = (index: number) => {
        if (items.length === 1) return;
        setItems(items.filter((_, i) => i !== index));
    };

    const totalAmount = items.reduce(
        (sum, item) => sum + item.quantity * item.unitPrice,
        0
    );

    const formatCurrency = (value: number) =>
        new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);

    const handleSubmit = async () => {
        if (!selectedCustomerId) {
            showToast('Please select a customer', 'warning');
            return;
        }
        const validItems = items.filter(
            (item) => item.productId !== '' && item.quantity > 0 && item.unitPrice > 0
        );
        if (validItems.length === 0) {
            showToast('Please add at least one valid item', 'warning');
            return;
        }

        setSaving(true);
        try {
            const orderItems: CreateOrderItemRequest[] = validItems.map((item) => ({
                productId: item.productId as number,
                quantity: item.quantity,
                unitPrice: item.unitPrice,
            }));

            await orderService.create({
                customerId: selectedCustomerId as number,
                items: orderItems,
            });
            showToast('Order created successfully');
            navigate('/orders');
        } catch {
            showToast('Failed to create order', 'error');
        } finally {
            setSaving(false);
        }
    };

    return (
        <Box>
            <Button
                startIcon={<ArrowBackIcon />}
                onClick={() => navigate('/orders')}
                sx={{ mb: 2 }}
            >
                Back to Orders
            </Button>

            <Typography variant="h4" sx={{ mb: 3 }}>
                New Order
            </Typography>

            <Paper sx={{ p: 3, mb: 3 }}>
                {/* Customer Select */}
                <FormControl fullWidth sx={{ mb: 3 }}>
                    <InputLabel>Customer</InputLabel>
                    <Select
                        value={selectedCustomerId}
                        label="Customer"
                        onChange={(e: SelectChangeEvent<number | ''>) =>
                            setSelectedCustomerId(e.target.value as number)
                        }
                    >
                        {customers.map((c) => (
                            <MenuItem key={c.id} value={c.id}>
                                {c.name} ({c.email})
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>

                <Divider sx={{ mb: 3 }} />

                {/* Items */}
                <Typography variant="h6" sx={{ mb: 2 }}>
                    Items
                </Typography>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell sx={{ width: '40%' }}>Product</TableCell>
                                <TableCell sx={{ width: '15%' }}>Quantity</TableCell>
                                <TableCell sx={{ width: '20%' }}>Unit Price</TableCell>
                                <TableCell sx={{ width: '15%' }}>Total</TableCell>
                                <TableCell sx={{ width: '10%' }} />
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {items.map((item, index) => (
                                <TableRow key={index}>
                                    <TableCell>
                                        <FormControl fullWidth size="small">
                                            <Select
                                                value={item.productId}
                                                displayEmpty
                                                onChange={(e: SelectChangeEvent<number | ''>) =>
                                                    handleProductChange(index, e.target.value as number)
                                                }
                                            >
                                                <MenuItem value="" disabled>
                                                    Select product...
                                                </MenuItem>
                                                {products.map((p) => (
                                                    <MenuItem key={p.id} value={p.id}>
                                                        {p.name} ({formatCurrency(p.price)})
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>
                                    </TableCell>
                                    <TableCell>
                                        <TextField
                                            type="number"
                                            size="small"
                                            value={item.quantity}
                                            onChange={(e) =>
                                                handleQuantityChange(index, parseInt(e.target.value) || 1)
                                            }
                                            slotProps={{ input: { inputProps: { min: 1 } } }}
                                            fullWidth
                                        />
                                    </TableCell>
                                    <TableCell>
                                        <TextField
                                            type="number"
                                            size="small"
                                            value={item.unitPrice}
                                            onChange={(e) =>
                                                handlePriceChange(index, parseFloat(e.target.value) || 0)
                                            }
                                            slotProps={{ input: { inputProps: { min: 0, step: 0.01 } } }}
                                            fullWidth
                                        />
                                    </TableCell>
                                    <TableCell sx={{ fontWeight: 500 }}>
                                        {formatCurrency(item.quantity * item.unitPrice)}
                                    </TableCell>
                                    <TableCell>
                                        <IconButton
                                            size="small"
                                            color="error"
                                            onClick={() => removeItem(index)}
                                            disabled={items.length === 1}
                                        >
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>

                <Box sx={{ mt: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Button startIcon={<AddIcon />} onClick={addItem} variant="outlined">
                        Add Item
                    </Button>
                    <Typography variant="h6" color="primary.main" sx={{ fontWeight: 700 }}>
                        Total: {formatCurrency(totalAmount)}
                    </Typography>
                </Box>
            </Paper>

            {/* Actions */}
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                <Button onClick={() => navigate('/orders')} size="large">
                    Cancel
                </Button>
                <Button
                    variant="contained"
                    size="large"
                    onClick={handleSubmit}
                    disabled={saving}
                >
                    {saving ? 'Creating...' : 'Create Order'}
                </Button>
            </Box>
        </Box>
    );
};

export default NewOrder;
