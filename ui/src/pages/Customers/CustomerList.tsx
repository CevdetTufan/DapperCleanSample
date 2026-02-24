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
    InputAdornment,
    Tooltip,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import SearchIcon from '@mui/icons-material/Search';
import { customerService } from '../../services/customerService';
import type { CustomerDto, CreateCustomerRequest, UpdateCustomerRequest } from '../../types';
import ConfirmDialog from '../../components/ConfirmDialog/ConfirmDialog';
import { useToast } from '../../components/Toast/ToastProvider';

const CustomerList: React.FC = () => {
    const { showToast } = useToast();
    const [customers, setCustomers] = useState<CustomerDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(0);
    const [pageSize] = useState(10);
    const [totalCount, setTotalCount] = useState(0);

    // Search
    const [searchEmail, setSearchEmail] = useState('');

    // Dialog
    const [dialogOpen, setDialogOpen] = useState(false);
    const [editingCustomer, setEditingCustomer] = useState<CustomerDto | null>(null);
    const [formData, setFormData] = useState<CreateCustomerRequest>({ name: '', email: '' });
    const [saving, setSaving] = useState(false);

    // Confirm delete
    const [deleteTarget, setDeleteTarget] = useState<CustomerDto | null>(null);

    const fetchCustomers = useCallback(async () => {
        setLoading(true);
        try {
            const result = await customerService.getPaged(page + 1, pageSize);
            setCustomers(result.items);
            setTotalCount(result.totalCount);
        } catch (error) {
            showToast(error instanceof Error ? error.message : 'Failed to load customers', 'error');
        } finally {
            setLoading(false);
        }
    }, [page, pageSize, showToast]);

    useEffect(() => {
        fetchCustomers();
    }, [fetchCustomers]);

    const handleSearchByEmail = async () => {
        if (!searchEmail.trim()) {
            fetchCustomers();
            return;
        }
        setLoading(true);
        try {
            const customer = await customerService.getByEmail(searchEmail.trim());
            setCustomers([customer]);
            setTotalCount(1);
        } catch {
            setCustomers([]);
            setTotalCount(0);
            showToast('Customer not found', 'warning');
        } finally {
            setLoading(false);
        }
    };

    const handleOpenCreate = () => {
        setEditingCustomer(null);
        setFormData({ name: '', email: '' });
        setDialogOpen(true);
    };

    const handleOpenEdit = (customer: CustomerDto) => {
        setEditingCustomer(customer);
        setFormData({ name: customer.name, email: customer.email });
        setDialogOpen(true);
    };

    const handleSave = async () => {
        if (!formData.name.trim() || !formData.email.trim()) {
            showToast('Please fill in all fields', 'warning');
            return;
        }
        setSaving(true);
        try {
            if (editingCustomer) {
                await customerService.update(editingCustomer.id, formData as UpdateCustomerRequest);
                showToast('Customer updated successfully');
            } else {
                await customerService.create(formData);
                showToast('Customer created successfully');
            }
            setDialogOpen(false);
            fetchCustomers();
        } catch (error) {
            showToast(error instanceof Error ? error.message : 'Failed to save customer', 'error');
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!deleteTarget) return;
        try {
            await customerService.delete(deleteTarget.id);
            showToast('Customer deleted successfully');
            setDeleteTarget(null);
            fetchCustomers();
        } catch (error) {
            showToast(error instanceof Error ? error.message : 'Failed to delete customer', 'error');
        }
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4">Customers</Typography>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={handleOpenCreate}
                >
                    New Customer
                </Button>
            </Box>

            {/* Search */}
            <Paper sx={{ p: 2, mb: 3, display: 'flex', gap: 2, alignItems: 'center' }}>
                <TextField
                    size="small"
                    placeholder="Search by email..."
                    value={searchEmail}
                    onChange={(e) => setSearchEmail(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSearchByEmail()}
                    slotProps={{
                        input: {
                            startAdornment: (
                                <InputAdornment position="start">
                                    <SearchIcon color="action" />
                                </InputAdornment>
                            ),
                        },
                    }}
                    sx={{ minWidth: 300 }}
                />
                <Button variant="outlined" onClick={handleSearchByEmail}>
                    Search
                </Button>
                {searchEmail && (
                    <Button
                        onClick={() => {
                            setSearchEmail('');
                            fetchCustomers();
                        }}
                    >
                        Clear
                    </Button>
                )}
            </Paper>

            {/* Table */}
            <Paper>
                <TableContainer>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>ID</TableCell>
                                <TableCell>Name</TableCell>
                                <TableCell>Email</TableCell>
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
                            ) : customers.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        <Typography color="text.secondary" sx={{ py: 4 }}>
                                            No customers found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                customers.map((c) => (
                                    <TableRow key={c.id} hover>
                                        <TableCell>{c.id}</TableCell>
                                        <TableCell sx={{ fontWeight: 500 }}>{c.name}</TableCell>
                                        <TableCell>{c.email}</TableCell>
                                        <TableCell>{new Date(c.createdAt).toLocaleDateString()}</TableCell>
                                        <TableCell align="right">
                                            <Tooltip title="Edit">
                                                <IconButton size="small" onClick={() => handleOpenEdit(c)}>
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Delete">
                                                <IconButton size="small" color="error" onClick={() => setDeleteTarget(c)}>
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
                <DialogTitle>{editingCustomer ? 'Edit Customer' : 'New Customer'}</DialogTitle>
                <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '16px !important' }}>
                    <TextField
                        label="Name"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                        fullWidth
                        required
                    />
                    <TextField
                        label="Email"
                        type="email"
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                        fullWidth
                        required
                    />
                </DialogContent>
                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
                    <Button variant="contained" onClick={handleSave} disabled={saving}>
                        {saving ? 'Saving...' : editingCustomer ? 'Update' : 'Create'}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Delete Confirmation */}
            <ConfirmDialog
                open={!!deleteTarget}
                title="Delete Customer"
                message={`Are you sure you want to delete "${deleteTarget?.name}"? This action cannot be undone.`}
                onConfirm={handleDelete}
                onCancel={() => setDeleteTarget(null)}
            />
        </Box>
    );
};

export default CustomerList;
