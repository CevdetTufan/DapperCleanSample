import React, { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
    Box,
    Drawer,
    AppBar,
    Toolbar,
    List,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Typography,
    IconButton,
    CssBaseline,
    Divider,
    useMediaQuery,
    useTheme,
} from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PeopleIcon from '@mui/icons-material/People';
import InventoryIcon from '@mui/icons-material/Inventory2';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';
import MenuIcon from '@mui/icons-material/Menu';
import StorefrontIcon from '@mui/icons-material/Storefront';

const DRAWER_WIDTH = 260;

const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
    { text: 'Customers', icon: <PeopleIcon />, path: '/customers' },
    { text: 'Products', icon: <InventoryIcon />, path: '/products' },
    { text: 'Orders', icon: <ShoppingCartIcon />, path: '/orders' },
];

const Layout: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [mobileOpen, setMobileOpen] = useState(false);

    const drawerContent = (
        <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
            <Box
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 1.5,
                    px: 2.5,
                    py: 2.5,
                }}
            >
                <StorefrontIcon sx={{ fontSize: 32, color: 'primary.main' }} />
                <Typography
                    variant="h6"
                    sx={{ fontWeight: 700, color: 'primary.main', letterSpacing: '-0.5px' }}
                >
                    DapperClean
                </Typography>
            </Box>
            <Divider />
            <List sx={{ px: 1.5, pt: 1.5, flex: 1 }}>
                {menuItems.map((item) => {
                    const isActive = location.pathname === item.path ||
                        (item.path !== '/' && location.pathname.startsWith(item.path));
                    return (
                        <ListItemButton
                            key={item.text}
                            onClick={() => {
                                navigate(item.path);
                                if (isMobile) setMobileOpen(false);
                            }}
                            selected={isActive}
                            sx={{
                                borderRadius: 2,
                                mb: 0.5,
                                '&.Mui-selected': {
                                    backgroundColor: 'primary.main',
                                    color: '#fff',
                                    '& .MuiListItemIcon-root': { color: '#fff' },
                                    '&:hover': { backgroundColor: 'primary.dark' },
                                },
                            }}
                        >
                            <ListItemIcon sx={{ minWidth: 40 }}>{item.icon}</ListItemIcon>
                            <ListItemText
                                primary={item.text}
                                primaryTypographyProps={{ fontWeight: isActive ? 600 : 400 }}
                            />
                        </ListItemButton>
                    );
                })}
            </List>
            <Divider />
            <Box sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="caption" color="text.secondary">
                    DapperCleanSample v1.0
                </Typography>
            </Box>
        </Box>
    );

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'grey.50' }}>
            <CssBaseline />

            {/* AppBar for mobile */}
            <AppBar
                position="fixed"
                elevation={0}
                sx={{
                    display: { md: 'none' },
                    bgcolor: 'background.paper',
                    borderBottom: '1px solid',
                    borderColor: 'divider',
                }}
            >
                <Toolbar>
                    <IconButton onClick={() => setMobileOpen(true)} edge="start">
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" color="text.primary" sx={{ fontWeight: 700 }}>
                        DapperClean
                    </Typography>
                </Toolbar>
            </AppBar>

            {/* Sidebar */}
            <Drawer
                variant={isMobile ? 'temporary' : 'permanent'}
                open={isMobile ? mobileOpen : true}
                onClose={() => setMobileOpen(false)}
                sx={{
                    width: DRAWER_WIDTH,
                    flexShrink: 0,
                    '& .MuiDrawer-paper': {
                        width: DRAWER_WIDTH,
                        boxSizing: 'border-box',
                        borderRight: '1px solid',
                        borderColor: 'divider',
                        bgcolor: 'background.paper',
                    },
                }}
            >
                {drawerContent}
            </Drawer>

            {/* Main content */}
            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    p: 3,
                    mt: { xs: 8, md: 0 },
                    width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
                }}
            >
                <Outlet />
            </Box>
        </Box>
    );
};

export default Layout;
