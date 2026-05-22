import { useEffect, useMemo, useState } from 'react'
import { AppShell } from './components/AppShell.jsx'
import { useAuth } from './hooks/useAuth.js'
import { useLocalStorage } from './hooks/useLocalStorage.js'
import { DashboardPage } from './pages/DashboardPage.jsx'
import { OrdersPage } from './pages/OrdersPage.jsx'
import { CreateOrderPage } from './pages/CreateOrderPage.jsx'
import { ProductsPage } from './pages/ProductsPage.jsx'
import { CreateProductPage } from './pages/CreateProductPage.jsx'
import { CustomersPage } from './pages/CustomersPage.jsx'
import { FinancesPage } from './pages/FinancesPage.jsx'
import { OperationsPage } from './pages/OperationsPage.jsx'
import { LoginPage } from './pages/LoginPage.jsx'
import { RegistrationPage } from './pages/RegistrationPage.jsx'
import { CreateCompanyPage } from './pages/CreateCompanyPage.jsx'
import { serverApi } from './lib/serverApi.js'

function formatDateTime(date) {
  return new Intl.DateTimeFormat('uk-UA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function normalizeId(value) {
  return Number(value)
}

function buildProductModels(serverProducts, variants, categories, warehouses) {
  return serverProducts.map((product) => {
    const productId = getValue(product, 'id', 'Id')
    const warehouseId = getValue(product, 'warehouseId', 'WarehouseId')
    const categoryIds = getValue(product, 'categoryIds', 'CategoryIds') ?? []
    const categoryId = categoryIds[0]
    const variant = variants.find((item) => getValue(item, 'productId', 'ProductId') === productId)
    const productCategories = categories.filter((item) => categoryIds.includes(getValue(item, 'id', 'Id')))
    const warehouse = warehouses.find((item) => getValue(item, 'id', 'Id') === warehouseId)

    return {
      id: productId,
      productId,
      variantId: getValue(variant, 'id', 'Id'),
      productCategoryId: categoryId,
      productCategoryIds: categoryIds,
      warehouseId,
      sku: getValue(variant, 'uniqCode', 'UniqCode') ?? `PRD-${productId}`,
      barcode: '',
      name: getValue(product, 'name', 'Name') ?? '',
      description: getValue(product, 'description', 'Description') ?? '',
      category: productCategories.map((item) => getValue(item, 'name', 'Name')).join(', ') || `Category #${categoryId}`,
      brand: '-',
      unit: 'одиниць',
      warehouse: getValue(warehouse, 'name', 'Name') ?? `Склад #${warehouseId}`,
      stock: Number(getValue(variant, 'stockQuantity', 'StockQuantity') ?? 0),
      cost: Number(getValue(variant, 'costPrice', 'CostPrice') ?? 0),
      price: Number(getValue(variant, 'sellPrice', 'SellPrice') ?? 0),
    }
  })
}

function buildCustomerModels(serverCustomers) {
  return serverCustomers.map((customer) => {
    const contact = getValue(customer, 'contact', 'Contact')
    const name = getValue(customer, 'name', 'Name') ?? ''
    const surname = getValue(customer, 'surname', 'Surname') ?? ''

    return {
      id: getValue(customer, 'id', 'Id'),
      name: `${name} ${surname}`.trim() || name,
      firstName: name,
      surname,
      email: getValue(customer, 'email', 'Email') ?? '',
      phone: getValue(contact, 'value', 'Value') ?? getValue(customer, 'contactValue', 'ContactValue') ?? '',
    }
  })
}

function buildOrderModels(serverOrders, customers, products) {
  return serverOrders.map((order) => {
    const orderItems = getValue(order, 'orderItems', 'OrderItems') ?? []
    const firstItem = orderItems[0]
    const productVariantId = getValue(firstItem, 'productVariantId', 'ProductVariantId')
    const product = products.find((item) => item.variantId === productVariantId)
    const customerId = getValue(order, 'customerId', 'CustomerId')
    const customer = customers.find((item) => item.id === customerId)
    const total = Number(getValue(order, 'totalCost', 'TotalCost') ?? 0)

    return {
      id: getValue(order, 'id', 'Id'),
      code: getValue(order, 'uniqCode', 'UniqCode') ?? '',
      customer: customer?.name ?? `Клієнт #${customerId}`,
      product: product?.name ?? 'Товар із замовлення',
      quantity: Number(getValue(firstItem, 'quantity', 'Quantity') ?? 0),
      total,
      cost: product ? product.cost : 0,
      profit: product ? total - product.cost : total,
      expense: 0,
      operationType: 'sale',
      account: 'Наложка NovaPay',
      channel: 'CRM',
      date: String(getValue(order, 'createdAt', 'CreatedAt') ?? '').slice(0, 10),
      comment: '',
      deliveryMode: getValue(order, 'address', 'Address') ? 'nova-post' : 'simple',
      status: getValue(order, 'status', 'Status'),
    }
  })
}

export function App() {
  const { user, login, logout } = useAuth()
  const [page, setPage] = useState('dashboard')
  const [orders, setOrders] = useState([])
  const [products, setProducts] = useState([])
  const [customers, setCustomers] = useState([])
  const [categories, setCategories] = useState([])
  const [warehouses, setWarehouses] = useState([])
  const [isLoading, setIsLoading] = useState(false)
  const [apiError, setApiError] = useState('')
  const [finances, setFinances] = useLocalStorage('midas-finances-v1', [])
  const [operations, setOperations] = useLocalStorage('midas-operations-v2', [])
  const [theme, setTheme] = useLocalStorage('midas-theme', 'light')
  const [requiresCompany, setRequiresCompany] = useState(false)
  const [isCheckingCompany, setIsCheckingCompany] = useState(false)
  const [isCreatingCompany, setIsCreatingCompany] = useState(false)
  const [companyError, setCompanyError] = useState('')

  async function loadServerData() {
    setIsLoading(true)
    setApiError('')

    try {
      const [
        serverProducts,
        serverVariants,
        serverCategories,
        serverWarehouses,
        serverCustomers,
      ] = await Promise.all([
        serverApi.products.getAll(),
        serverApi.productVariants.getAll(),
        serverApi.categories.getAll(),
        serverApi.warehouses.getAll(),
        serverApi.customers.getAll(),
      ])
      const nextCustomers = buildCustomerModels(serverCustomers)
      const nextProducts = buildProductModels(serverProducts, serverVariants, serverCategories, serverWarehouses)
      const serverOrders = await serverApi.orders.getAll()

      setCategories(serverCategories)
      setWarehouses(serverWarehouses)
      setCustomers(nextCustomers)
      setProducts(nextProducts)
      setOrders(buildOrderModels(serverOrders, nextCustomers, nextProducts))
    } catch (error) {
      setApiError(error.message || 'Не вдалося завантажити дані з сервера')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    if (user?.token) {
      Promise.resolve().then(async () => {
        setIsCheckingCompany(true)
        setCompanyError('')

        try {
          await serverApi.companies.getBalance({ handleUnauthorized: false })
          setRequiresCompany(false)
          await loadServerData()
        } catch (error) {
          if (error.status === 401) {
            setRequiresCompany(true)
            return
          }

          setApiError(error.message || 'Не вдалося перевірити доступ до компанії')
        } finally {
          setIsCheckingCompany(false)
        }
      })
    } else {
      setRequiresCompany(false)
      setCompanyError('')
    }
  }, [user?.token])

  async function createCompany(companyPayload) {
    setIsCreatingCompany(true)
    setCompanyError('')

    try {
      await serverApi.companies.create(companyPayload)
      setRequiresCompany(false)
      await loadServerData()
    } catch (error) {
      setCompanyError(error.message || 'Не вдалося створити компанію')
    } finally {
      setIsCreatingCompany(false)
    }
  }

  const stats = useMemo(
    () => {
      const revenue = orders.reduce((sum, order) => sum + order.total, 0)
      const grossProfit = orders.reduce((sum, order) => sum + order.profit, 0)
      const saleExpenses = orders.reduce((sum, order) => sum + order.expense, 0)
      const manualExpenses = finances.reduce((sum, finance) => sum + Number(finance.amount), 0)
      const totalExpenses = saleExpenses + manualExpenses

      return {
        sales: orders.length,
        customers: customers.length,
        products: products.length,
        expensesCount: finances.length,
        revenue,
        grossProfit,
        expenses: totalExpenses,
        loss: totalExpenses,
      }
    },
    [customers.length, finances, orders, products.length],
  )

  function addOperation(operation) {
    setOperations((currentOperations) => [
      {
        id: crypto.randomUUID(),
        createdAt: formatDateTime(new Date()),
        actor: user?.email ?? 'system',
        ...operation,
      },
      ...currentOperations,
    ])
  }

  async function addSale(sale) {
    const selectedCustomer = customers.find((customer) => customer.id === normalizeId(sale.customerId))
    const selectedProduct = products.find((product) => product.id === normalizeId(sale.productId))

    if (!selectedCustomer) {
      throw new Error('Оберіть клієнта з бази даних')
    }

    if (!selectedProduct?.variantId) {
      throw new Error('Для цього товару немає варіанту на сервері, тому продаж неможливо створити')
    }

    await serverApi.orders.createOneClick({
      customer: {
        name: selectedCustomer.firstName || selectedCustomer.name,
        surname: selectedCustomer.surname || '-',
        contactValue: selectedCustomer.phone || '+380000000000',
        email: selectedCustomer.email || 'customer@midas.local',
      },
      address: {
        city: sale.city || 'Київ',
        postalCode: Number(sale.postalCode) || 1,
        postDepartmentNumber: Number(sale.postDepartmentNumber) || 1,
      },
      serviceType: Number(sale.serviceType),
      cargoType: Number(sale.cargoType),
      description: sale.description || selectedProduct.name,
      paymentMethods: Number(sale.paymentMethods),
      items: [
        {
          productVariantId: selectedProduct.variantId,
          quantity: Number(sale.quantity) || 1,
        },
      ],
    })

    await loadServerData()
    setPage('orders')
  }

  function addFinances(finance) {
    setFinances((currentFinances) => [
      {
        ...finance,
        id: crypto.randomUUID(),
      },
      ...currentFinances,
    ])

    addOperation({
      type: 'Фінансова витрата',
      description: finance.description || 'Додано витрату',
      amount: `${Number(finance.amount).toLocaleString('uk-UA')} грн`,
    })
  }

  async function addProduct(product) {
    if (!product.warehouseId || !product.productCategoryIds?.length) {
      throw new Error('На сервері потрібні склад і категорія для створення товару')
    }

    await serverApi.products.createWithVariants({
      warehouseId: normalizeId(product.warehouseId),
      name: product.name,
      description: product.description,
      weight: Number(product.weight) || 0,
      productCategoryIds: product.productCategoryIds.map(normalizeId),
      variants: [
        {
          color: product.color || '-',
          size: product.size || '-',
          quantity: Number(product.stock) || 0,
          costPrice: Number(product.cost) || 0,
          sellPrice: Number(product.price) || 0,
        },
      ],
    })

    await loadServerData()
    setPage('products')
  }

  if (!user) {
    return <LoginPage onLogin={login} />
  }

  if (isCheckingCompany) {
    return (
      <main className="login-page">
        <div className="login-card">
          <h1>Перевірка доступу...</h1>
          <p className="create-company-subtitle">Зачекайте, будь ласка. Перевіряємо участь у компанії.</p>
        </div>
      </main>
    )
  }

  if (requiresCompany) {
    return (
      <CreateCompanyPage
        userEmail={user.email}
        onCreateCompany={createCompany}
        isSubmitting={isCreatingCompany}
        error={companyError}
        onLogout={logout}
      />
    )
  }

  const pages = {
    dashboard: (
      <DashboardPage
        stats={stats}
        sales={orders}
        recentSales={orders.slice(0, 3)}
        operations={operations.slice(0, 5)}
      />
    ),
    orders: <OrdersPage orders={orders} onNavigate={setPage} />,
    createOrder: (
      <CreateOrderPage
        customers={customers}
        products={products}
        onBack={() => setPage('orders')}
        onCreate={addSale}
      />
    ),
    products: <ProductsPage products={products} onNavigate={setPage} />,
    createProduct: (
      <CreateProductPage
        categories={categories}
        warehouses={warehouses}
        onBack={() => setPage('products')}
        onCreate={addProduct}
      />
    ),
    finances: <FinancesPage finances={finances} onCreate={addFinances} />,
    customers: <CustomersPage customers={customers} />,
    operations: <OperationsPage operations={operations} />,
  }

  return (
    <AppShell activePage={page} user={user} theme={theme} onThemeChange={setTheme} onNavigate={setPage} onLogout={logout}>
      {apiError && (
        <div className="api-error-banner">
          <span>{apiError}</span>
          <button type="button" onClick={loadServerData}>Повторити</button>
        </div>
      )}
      {isLoading && <div className="api-info-banner">Завантаження даних з сервера...</div>}
      {pages[page]}
    </AppShell>
  )
}


