import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import theme from './theme';
import { ToastProvider } from './components/Toast/ToastProvider';
import Layout from './components/Layout/Layout';
import Dashboard from './pages/Dashboard/Dashboard';
import CustomerList from './pages/Customers/CustomerList';
import ProductList from './pages/Products/ProductList';
import OrderList from './pages/Orders/OrderList';
import OrderDetail from './pages/Orders/OrderDetail';
import NewOrder from './pages/Orders/NewOrder';

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ToastProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Layout />}>
              <Route index element={<Dashboard />} />
              <Route path="customers" element={<CustomerList />} />
              <Route path="products" element={<ProductList />} />
              <Route path="orders" element={<OrderList />} />
              <Route path="orders/new" element={<NewOrder />} />
              <Route path="orders/:id" element={<OrderDetail />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </ToastProvider>
    </ThemeProvider>
  );
}

export default App;
