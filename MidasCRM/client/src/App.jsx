import { useEffect, useMemo, useState } from 'react'
import { AppShell } from './components/AppShell.jsx'
import { useAuth } from './hooks/useAuth.js'
import { useLocalStorage } from './hooks/useLocalStorage.js'
import { DashboardPage } from './pages/dashboard/DashboardPage.jsx'
import { OrdersPage } from './pages/sales/OrdersPage.jsx'
import { CreateOrderPage } from './pages/sales/CreateOrderPage.jsx'
import { ProductsPage } from './pages/products/ProductsPage.jsx'
import { CreateProductPage } from './pages/products/CreateProductPage.jsx'
import { CustomersPage } from './pages/customers/CustomersPage.jsx'
import { FinancesPage } from './pages/finance/FinancesPage.jsx'
import { OperationsPage } from './pages/operations/OperationsPage.jsx'
import { LoginPage } from './pages/auth/LoginPage.jsx'
import { RegistrationPage } from './pages/auth/RegistrationPage.jsx'
import { CreateCompanyPage } from './pages/company/CreateCompanyPage.jsx'
import { CompanyPage } from './pages/company/CompanyPage.jsx'
import { serverApi } from './lib/serverApi.js'
import authStyles from './features/auth/styles/Auth.module.css'
import companyStyles from './styles/pages/Company.module.css'

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

function getCompanyId(company) {
  return String(getValue(company, 'id', 'Id'))
}

function getCompanyMembers(company) {
  return getValue(company, 'members', 'Members') ?? []
}

function getCurrentUserCompanyRole(company, userId) {
  const member = getCompanyMembers(company).find((item) => String(getValue(item, 'userId', 'UserId')) === String(userId))
  return Number(getValue(member, 'role', 'Role') ?? 0)
}

export function App() {
  const { user, login, register, logout } = useAuth()
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
  const [theme, setTheme] = useLocalStorage('midas-theme', 'dark')
  const [companies, setCompanies] = useState([])
  const [activeCompanyId, setActiveCompanyId] = useLocalStorage('midas-active-company-id', null)
  const [requiresCompany, setRequiresCompany] = useState(false)
  const [isCheckingCompany, setIsCheckingCompany] = useState(false)
  const [isCreatingCompany, setIsCreatingCompany] = useState(false)
  const [companyError, setCompanyError] = useState('')
  const [companyPageError, setCompanyPageError] = useState('')
  const [isCompanyActionLoading, setIsCompanyActionLoading] = useState(false)

  useEffect(() => {
    document.documentElement.dataset.theme = theme === 'dark' ? 'dark' : 'light'
  }, [theme])

  async function loadServerData() {
    setIsLoading(true)
    setApiError('')

    try {
      const [serverProducts, serverVariants, serverCategories, serverWarehouses, serverCustomers] = await Promise.all([
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

  async function loadCompaniesAndBootstrap() {
    setIsCheckingCompany(true)
    setCompanyError('')

    try {
      const serverCompanies = await serverApi.companies.getAll()
      setCompanies(serverCompanies)

      if (!serverCompanies.length) {
        setRequiresCompany(true)
        return
      }

      setRequiresCompany(false)

      const nextActiveCompanyId = serverCompanies.some((company) => getCompanyId(company) === String(activeCompanyId))
        ? String(activeCompanyId)
        : getCompanyId(serverCompanies[0])

      localStorage.setItem('midas-active-company-id', nextActiveCompanyId)
      setActiveCompanyId(nextActiveCompanyId)
      await loadServerData()
    } catch (error) {
      if (error.status === 401) {
        setRequiresCompany(true)
      } else {
        setApiError(error.message || 'Не вдалося перевірити доступ до компаній')
      }
    } finally {
      setIsCheckingCompany(false)
    }
  }

  useEffect(() => {
    if (!user?.token) {
      return
    }

    Promise.resolve().then(loadCompaniesAndBootstrap)
    // Company bootstrap should rerun only when the auth token changes.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.token])

  async function createCompany(companyPayload) {
    setIsCreatingCompany(true)
    setCompanyError('')

    try {
      await serverApi.companies.create(companyPayload)
      await loadCompaniesAndBootstrap()
    } catch (error) {
      setCompanyError(error.message || 'Не вдалося створити компанію')
    } finally {
      setIsCreatingCompany(false)
    }
  }

  async function switchActiveCompany(nextCompanyId) {
    if (!nextCompanyId || String(nextCompanyId) === String(activeCompanyId)) {
      return
    }

    setApiError('')
    setIsLoading(true)

    localStorage.setItem('midas-active-company-id', String(nextCompanyId))
    setActiveCompanyId(String(nextCompanyId))

    try {
      await loadServerData()
      setPage('dashboard')
    } finally {
      setIsLoading(false)
    }
  }

  const activeCompany = useMemo(
    () => companies.find((company) => getCompanyId(company) === String(activeCompanyId)) ?? companies[0] ?? null,
    [companies, activeCompanyId],
  )

  const currentUserCompanyRole = useMemo(
    () => getCurrentUserCompanyRole(activeCompany, user?.id),
    [activeCompany, user?.id],
  )

  const stats = useMemo(() => {
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
  }, [customers.length, finances, orders, products.length])

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

    if (!product.variants?.length) {
      throw new Error('Додайте хоча б один варіант товару')
    }

    const createdProduct = await serverApi.products.createWithVariants({
      warehouseId: normalizeId(product.warehouseId),
      name: product.name,
      description: product.description,
      weight: Number(product.weight) || 0,
      productCategoryIds: product.productCategoryIds.map(normalizeId),
      variants: product.variants.map((variant) => ({
        color: variant.color || '-',
        size: variant.size || '-',
        quantity: Number(variant.quantity) || 0,
        costPrice: Number(variant.costPrice) || 0,
        sellPrice: Number(variant.sellPrice) || 0,
      })),
    })

    let uploadedCount = 0
    let failedCount = 0
    let mainImageResponse = null

    if (product.images?.length) {
      const uploadedImages = []

      for (const imageFile of product.images) {
        try {
          const imageResponse = await serverApi.products.addImage(createdProduct.id, imageFile)
          uploadedImages.push(imageResponse)
          uploadedCount += 1
        } catch {
          failedCount += 1
        }
      }

      const canSetMainImage =
        Number.isInteger(product.mainImageIndex) &&
        product.mainImageIndex >= 0 &&
        product.mainImageIndex < uploadedImages.length

      if (canSetMainImage) {
        try {
          mainImageResponse = await serverApi.products.setMainImage(createdProduct.id, uploadedImages[product.mainImageIndex].id)
        } catch {
          failedCount += 1
        }
      }
    }

    await loadServerData()
    setPage('products')

    return {
      productId: createdProduct.id,
      uploadedCount,
      failedCount,
      hasMainImage: Boolean(mainImageResponse),
    }
  }

  async function createWarehouse(payload) {
    await serverApi.warehouses.create(payload)
    await loadServerData()
  }

  async function updateWarehouse(id, payload) {
    await serverApi.warehouses.update(id, payload)
    await loadServerData()
  }

  async function runCompanyAction(action) {
    setIsCompanyActionLoading(true)
    setCompanyPageError('')

    try {
      await action()
      const reloadedCompanies = await serverApi.companies.getAll()
      setCompanies(reloadedCompanies)
      await loadServerData()
    } catch (error) {
      setCompanyPageError(error.message || 'Помилка під час зміни компанії')
    } finally {
      setIsCompanyActionLoading(false)
    }
  }

  async function handleCreateCompanyFromPage(payload) {
    await runCompanyAction(async () => {
      const createdCompany = await serverApi.companies.create(payload)
      const createdCompanyId = getCompanyId(createdCompany)
      localStorage.setItem('midas-active-company-id', createdCompanyId)
      setActiveCompanyId(createdCompanyId)
    })
  }

  async function handleUpdateCompany(payload) {
    if (!activeCompany) {
      return
    }

    await runCompanyAction(() => serverApi.companies.update(getCompanyId(activeCompany), payload))
  }

  async function handleDeleteCompany() {
    if (!activeCompany) {
      return
    }

    await runCompanyAction(async () => {
      await serverApi.companies.remove(getCompanyId(activeCompany))
      const reloadedCompanies = await serverApi.companies.getAll()
      if (!reloadedCompanies.length) {
        localStorage.removeItem('midas-active-company-id')
        setActiveCompanyId(null)
        setRequiresCompany(true)
      } else {
        const nextActiveCompanyId = getCompanyId(reloadedCompanies[0])
        localStorage.setItem('midas-active-company-id', nextActiveCompanyId)
        setActiveCompanyId(nextActiveCompanyId)
      }
    })
  }

  async function handleAddCompanyMember(email) {
    if (!activeCompany) {
      return
    }

    await runCompanyAction(() => serverApi.companies.addMemberByEmail(getCompanyId(activeCompany), email))
  }

  async function handleChangeCompanyMemberRole(userId, role) {
    await runCompanyAction(() => serverApi.companyMembers.updateRole(userId, role))
  }

  async function handleRemoveCompanyMember(userId) {
    await runCompanyAction(() => serverApi.companyMembers.remove(userId))
  }

  if (!user) {
    const currentPath = window.location.pathname.toLowerCase()
    if (currentPath === '/register') {
      return <RegistrationPage onRegister={register} theme={theme} />
    }

    return <LoginPage onLogin={login} theme={theme} />
  }

  if (isCheckingCompany) {
    return (
      <main className={authStyles['login-page']} data-theme={theme === 'dark' ? 'dark' : 'light'}>
        <div className={authStyles['login-card']}>
          <h1>Завантажуємо дані</h1>
          <p className={companyStyles['create-company-subtitle']}>Перевіряємо доступ до компаній і готуємо робочий простір.</p>
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
    products: (
      <ProductsPage
        products={products}
        warehouses={warehouses}
        onNavigate={setPage}
        onCreateWarehouse={createWarehouse}
        onUpdateWarehouse={updateWarehouse}
      />
    ),
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
    company: (
      <CompanyPage
        activeCompany={activeCompany}
        currentUserId={user?.id}
        onCreateCompany={handleCreateCompanyFromPage}
        onUpdateCompany={handleUpdateCompany}
        onDeleteCompany={handleDeleteCompany}
        onAddMember={handleAddCompanyMember}
        onChangeMemberRole={handleChangeCompanyMemberRole}
        onRemoveMember={handleRemoveCompanyMember}
        isBusy={isCompanyActionLoading}
        error={companyPageError}
      />
    ),
  }

  return (
    <AppShell
      activePage={page}
      user={user}
      theme={theme}
      onThemeChange={setTheme}
      onNavigate={setPage}
      onLogout={logout}
      companies={companies}
      activeCompanyId={activeCompany ? getCompanyId(activeCompany) : null}
      activeCompanyRole={currentUserCompanyRole}
      onCompanyChange={switchActiveCompany}
    >
      {apiError && (
        <div className="api-error-banner">
          <span>{apiError}</span>
          <button type="button" onClick={loadServerData}>Повторити</button>
        </div>
      )}
      {isLoading && <div className="api-info-banner">Почекайте будь ласка, завантажуємо дані</div>}
      {pages[page]}
    </AppShell>
  )
}

