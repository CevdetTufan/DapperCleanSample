import React, { useState, useEffect, useCallback } from 'react';
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
    TextField,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Skeleton,
    Tooltip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { productService } from '../../services/productService';
import type { ProductDto, CreateProductRequest, UpdateProductRequest } from '../../types';
import ConfirmDialog from '../../components/ConfirmDialog/ConfirmDialog';
import { useToast } from '../../components/Toast/ToastProvider';

const ProductList: React.FC = () => {
    const { showToast } = useToast();
    const [products, setProducts] = useState<ProductDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(0);
    const [pageSize] = useState(10);
    const [totalCount, setTotalCount] = useState(0);

    // Dialog
    const [dialogOpen, setDialogOpen] = useState(false);
    const [editingProduct, setEditingProduct] = useState<ProductDto | null>(null);
    const [formData, setFormData] = useState<CreateProductRequest>({ name: '', price: 0 });
    const [saving, setSaving] = useState(false);

    // Confirm delete
    const [deleteTarget, setDeleteTarget] = useState<ProductDto | null>(null);

    const fetchProducts = useCallback(async () => {
        setLoading(true);
        try {
            const result = await productService.getPaged(page + 1, pageSize);
            setProducts(result.items);
            setTotalCount(result.totalCount);
        } catch (error) {
            showToast('Failed to load products', 'error');
            console.error(error);
        } finally {
            setLoading(false);
        }
    }, [page, pageSize, showToast]);

    useEffect(() => {
        fetchProducts();
    }, [fetchProducts]);

    const handleOpenCreate = () => {
        setEditingProduct(null);
        setFormData({ name: '', price: 0 });
        setDialogOpen(true);
    };

    const handleOpenEdit = (product: ProductDto) => {
        setEditingProduct(product);
        setFormData({ name: product.name, price: product.price });
        setDialogOpen(true);
    };

    const handleSave = async () => {
        if (!formData.name.trim()) {
            showToast('Please enter a product name', 'warning');
            return;
        }
        if (formData.price <= 0) {
            showToast('Price must be greater than 0', 'warning');
            return;
        }
        setSaving(true);
        try {
            if (editingProduct) {
                await productService.update(editingProduct.id, formData as UpdateProductRequest);
                showToast('Product updated successfully');
            } else {
                await productService.create(formData);
                showToast('Product created successfully');
            }
            setDialogOpen(false);
            fetchProducts();
        } catch {
            showToast('Failed to save product', 'error');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!deleteTarget) return;
        try {
            await productService.delete(deleteTarget.id);
            showToast('Product deleted successfully');
            setDeleteTarget(null);
            fetchProducts();
        } catch {
            showToast('Failed to delete product', 'error');
        }
    };

    const formatCurrency = (value: number) =>
        new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);

    return (
        <Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4">Products</Typography>
                <Button variant="contained" startIcon={<AddIcon />} onClick={handleOpenCreate}>
                    New Product
                </Button>
            </Box>

            {/* Table */}
            <Paper>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>ID</TableCell>
                                <TableCell>Name</TableCell>
                                <TableCell>Price</TableCell>
                                <TableCell>Created At</TableCell>
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
                            ) : products.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        <Typography color="text.secondary" sx={{ py: 4 }}>
                                            No products found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                products.map((p) => (
                                    <TableRow key={p.id} hover>
                                        <TableCell>{p.id}</TableCell>
                                        <TableCell sx={{ fontWeight: 500 }}>{p.name}</TableCell>
                                        <TableCell>{formatCurrency(p.price)}</TableCell>
                                        <TableCell>{new Date(p.createdAt).toLocaleDateString()}</TableCell>
                                        <TableCell align="right">
                                            <Tooltip title="Edit">
                                                <IconButton size="small" onClick={() => handleOpenEdit(p)}>
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Delete">
                                                <IconButton size="small" color="error" onClick={() => setDeleteTarget(p)}>
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

            {/* Create/Edit Dialog */}
            <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle>{editingProduct ? 'Edit Product' : 'New Product'}</DialogTitle>
                <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
                    <TextField
                        label="Name"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                        fullWidth
                        required
                    />
                    <TextField
                        label="Price"
                        type="number"
                        value={formData.price}
                        onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) || 0 })}
                        fullWidth
                        required
                        slotProps={{ input: { inputProps: { min: 0, step: 0.01 } } }}
                    />
                </DialogContent>
                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
                    <Button variant="contained" onClick={handleSave} disabled={saving}>
                        {saving ? 'Saving...' : editingProduct ? 'Update' : 'Create'}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Delete Confirmation */}
            <ConfirmDialog
                open={!!deleteTarget}
                title="Delete Product"
                message={`Are you sure you want to delete "${deleteTarget?.name}"? This action cannot be undone.`}
                onConfirm={handleDelete}
                onCancel={() => setDeleteTarget(null)}
            />
        </Box>
    );
};

export default ProductList;
