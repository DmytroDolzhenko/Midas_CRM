import { useMemo, useState } from 'react'
import { AppShell } from './components/AppShell.jsx'
import { useAuth } from './hooks/useAuth.js'
import { useLocalStorage } from './hooks/useLocalStorage.js'
import { DashboardPage } from './pages/DashboardPage.jsx'
import { OrdersPage } from './pages/OrdersPage.jsx'
import { CreateOrderPage } from './pages/CreateOrderPage.jsx'
import { ProductsPage } from './pages/ProductsPage.jsx'
import { CreateProductPage } from './pages/CreateProductPage.jsx'
import { CustomersPage } from './pages/CustomersPage.jsx'
import { ChatsPage } from './pages/ChatsPage.jsx'
import { LoginPage } from './pages/LoginPage.jsx'
import { initialChats, initialCustomers, initialOrders, initialProducts } from './lib/mockData.js'

export function App() {
  const { user, login, logout } = useAuth()
  const [page, setPage] = useState('dashboard')
  const [orders, setOrders] = useLocalStorage('midas-orders', initialOrders)
  const [products, setProducts] = useLocalStorage('midas-products', initialProducts)
  const [customers] = useLocalStorage('midas-customers', initialCustomers)
  const [chats] = useLocalStorage('midas-chats', initialChats)

  const stats = useMemo(
    () => ({
      orders: orders.length,
      customers: customers.length,
      products: products.length,
      chats: chats.length,
      unreadMessages: chats.reduce((sum, chat) => sum + chat.unread, 0),
      revenue: orders.reduce((sum, order) => sum + order.total, 0),
    }),
    [chats, customers.length, orders, products.length],
  )

  function addOrder(order) {
    setOrders((currentOrders) => [
      {
        ...order,
        id: crypto.randomUUID(),
        code: `MD-${1000 + currentOrders.length + 1}`,
        status: 'processing',
      },
      ...currentOrders,
    ])
    setPage('orders')
  }

  function addProduct(product) {
    setProducts((currentProducts) => [
      {
        ...product,
        id: crypto.randomUUID(),
        sku: `PRD-${currentProducts.length + 11}`,
      },
      ...currentProducts,
    ])
    setPage('products')
  }

  if (!user) {
    return <LoginPage onLogin={login} />
  }

  const pages = {
    dashboard: (
      <DashboardPage stats={stats} onNavigate={setPage} recentOrders={orders.slice(0, 3)} />
    ),
    orders: <OrdersPage orders={orders} onNavigate={setPage} />,
    createOrder: (
      <CreateOrderPage
        customers={customers}
        products={products}
        onBack={() => setPage('orders')}
        onCreate={addOrder}
      />
    ),
    products: <ProductsPage products={products} onNavigate={setPage} />,
    createProduct: <CreateProductPage onBack={() => setPage('products')} onCreate={addProduct} />,
    customers: <CustomersPage customers={customers} />,
    chats: <ChatsPage chats={chats} />,
  }

  return (
    <AppShell activePage={page} user={user} onNavigate={setPage} onLogout={logout}>
      {pages[page]}
    </AppShell>
  )
}
