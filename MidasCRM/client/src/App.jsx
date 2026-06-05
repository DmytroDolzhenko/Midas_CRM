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
import { CreateCustomerPage } from './pages/customers/CreateCustomerPage.jsx'
import { FinancesPage } from './pages/finance/FinancesPage.jsx'
import { OperationsPage } from './pages/operations/OperationsPage.jsx'
import { NovaPoshtaIntegrationPage } from './pages/Integrations/NovaPoshtaIntegrationPage.jsx'
import { NovaPoshtaLogisticProfilePage } from './pages/Integrations/NovaPoshtaLogisticProfilePage.jsx'
import { OlxIntegrationPage } from './pages/Integrations/OlxIntegrationPage.jsx'
import { ProfilePage } from './pages/profile/ProfilePage.jsx'
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

function getImageUrl(image) {
  return getValue(image, 'url', 'Url') ?? ''
}

function getProductImages(product) {
  return getValue(product, 'images', 'Images') ?? []
}

function getVariantReservedQuantity(orders, variantId) {
  return orders
    .filter((order) => !getValue(order, 'isDeleted', 'IsDeleted'))
    .filter((order) => ![4, 6].includes(Number(getValue(order, 'status', 'Status'))))
    .flatMap((order) => getValue(order, 'orderItems', 'OrderItems') ?? [])
    .filter((item) => !getValue(item, 'isDeleted', 'IsDeleted'))
    .filter((item) => Number(getValue(item, 'productVariantId', 'ProductVariantId')) === Number(variantId))
    .reduce((sum, item) => sum + Number(getValue(item, 'quantity', 'Quantity') ?? 0), 0)
}

function buildProductModels(serverProducts, variants, categories, warehouses, serverOrders = []) {
  return serverProducts.map((product) => {
    const productId = getValue(product, 'id', 'Id')
    const warehouseId = getValue(product, 'warehouseId', 'WarehouseId')
    const categoryIds = getValue(product, 'categoryIds', 'CategoryIds')
      ?? getValue(product, 'productCategoryIds', 'ProductCategoryIds')
      ?? []
    const categoryId = categoryIds[0]
    const images = getProductImages(product)
      .map((image) => ({
        id: getValue(image, 'id', 'Id'),
        url: getImageUrl(image),
        isMain: Boolean(getValue(image, 'isMain', 'IsMain')),
      }))
      .filter((image) => image.url)
    const productVariants = variants
      .filter((item) => getValue(item, 'productId', 'ProductId') === productId)
      .map((item) => {
        const variantId = getValue(item, 'id', 'Id')
        const stockQuantity = Number(getValue(item, 'stockQuantity', 'StockQuantity') ?? 0)
        const reservedQuantity = getVariantReservedQuantity(serverOrders, variantId)

        return {
          id: variantId,
          uniqCode: getValue(item, 'uniqCode', 'UniqCode'),
          color: getValue(item, 'color', 'Color'),
          size: getValue(item, 'size', 'Size'),
          stockQuantity: Math.max(stockQuantity - reservedQuantity, 0),
          originalStockQuantity: stockQuantity,
          reservedQuantity,
          costPrice: Number(getValue(item, 'costPrice', 'CostPrice') ?? 0),
          sellPrice: Number(getValue(item, 'sellPrice', 'SellPrice') ?? 0),
        }
      })
    const variant = productVariants[0]
    const productCategories = categories.filter((item) => categoryIds.includes(getValue(item, 'id', 'Id')))
    const warehouse = warehouses.find((item) => getValue(item, 'id', 'Id') === warehouseId)
    const categoryNames = productCategories.map((item) => getValue(item, 'name', 'Name')).filter(Boolean)
    const mainImage = images.find((image) => image.isMain) ?? images[0]

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
      category: categoryNames.join(', ') || 'Без категорії',
      brand: '-',
      unit: 'одиниць',
      warehouse: getValue(warehouse, 'name', 'Name') ?? `Склад #${warehouseId}`,
      stock: productVariants.reduce((sum, item) => sum + item.stockQuantity, 0),
      cost: Number(variant?.costPrice ?? 0),
      price: Number(variant?.sellPrice ?? 0),
      variants: productVariants,
      images,
      imageUrl: mainImage?.url ?? '',
    }
  })
}

function buildCustomerModels(serverCustomers) {
  return serverCustomers.filter((customer) => !getValue(customer, 'isDeleted', 'IsDeleted')).map((customer) => {
    const contact = getValue(customer, 'contact', 'Contact')
    const name = getValue(customer, 'name', 'Name') ?? ''
    const surname = getValue(customer, 'surname', 'Surname') ?? ''

    return {
      id: getValue(customer, 'id', 'Id'),
      name: `${name} ${surname}`.trim() || name,
      firstName: name,
      surname,
      email: getValue(customer, 'email', 'Email') ?? '',
      phone: getValue(contact, 'phoneNumber', 'PhoneNumber')
        ?? getValue(contact, 'value', 'Value')
        ?? getValue(customer, 'contactValue', 'ContactValue')
        ?? '',
      isDeleted: Boolean(getValue(customer, 'isDeleted', 'IsDeleted')),
    }
  })
}

function buildOrderModels(serverOrders, customers, products) {
  const variantsIndex = new Map()
  products.forEach((product) => {
    product.variants.forEach((variant) => {
      variantsIndex.set(variant.id, {
        productName: product.name,
        uniqCode: variant.uniqCode,
        color: variant.color,
        size: variant.size,
      })
    })
  })

  return serverOrders.map((order) => {
    const orderItems = getValue(order, 'orderItems', 'OrderItems') ?? []
    const activeOrderItems = orderItems.filter((item) => !getValue(item, 'isDeleted', 'IsDeleted'))
    const firstItem = activeOrderItems[0]
    const productVariantId = getValue(firstItem, 'productVariantId', 'ProductVariantId')
    const product = products.find((item) => item.variants?.some((variant) => variant.id === productVariantId))
    const customerId = getValue(order, 'customerId', 'CustomerId')
    const customer = customers.find((item) => item.id === customerId)
    const customerName = customer ? `${customer.firstName || customer.name || ''} ${customer.surname || ''}`.trim() : `Клієнт #${customerId}`
    const total = Number(getValue(order, 'totalCost', 'TotalCost') ?? 0)
    const cost = activeOrderItems.reduce((sum, item) => (
      sum + Number(getValue(item, 'quantity', 'Quantity') ?? 0) * Number(getValue(item, 'costPriceSnapshot', 'CostPriceSnapshot') ?? 0)
    ), 0)
    const quantity = activeOrderItems.reduce((sum, item) => sum + Number(getValue(item, 'quantity', 'Quantity') ?? 0), 0)

    return {
      id: getValue(order, 'id', 'Id'),
      code: getValue(order, 'uniqCode', 'UniqCode') ?? '',
      companyId: getValue(order, 'companyId', 'CompanyId'),
      customer: customer?.name ?? `Клієнт #${customerId}`,
      product: product?.name ?? 'Товар із замовлення',
      quantity,
      total,
      cost,
      profit: total - cost,
      expense: 0,
      operationType: 'sale',
      account: 'Наложка NovaPay',
      channel: 'CRM',
      date: String(getValue(order, 'createdAt', 'CreatedAt') ?? '').slice(0, 10),
      comment: getValue(order, 'description', 'Description') ?? '',
      deliveryMode: getValue(order, 'address', 'Address') ? 'nova-post' : 'simple',
      status: getValue(order, 'status', 'Status'),
      trackingNumber: getValue(order, 'trackingNumber', 'TrackingNumber') ?? '',
      customerDetails: {
        name: customer?.firstName ?? customer?.name ?? '',
        surname: customer?.surname ?? '',
      },
      items: orderItems.map((item) => {
        const variantId = getValue(item, 'productVariantId', 'ProductVariantId')
        const variantInfo = variantsIndex.get(variantId)
        const quantity = Number(getValue(item, 'quantity', 'Quantity') ?? 0)
        const unitPrice = Number(getValue(item, 'unitPrice', 'UnitPrice') ?? 0)
        return {
          id: getValue(item, 'id', 'Id') ?? `${variantId}-${quantity}`,
          productVariantId: variantId,
          productName: variantInfo?.productName ?? `Variant #${variantId}`,
          uniqCode: variantInfo?.uniqCode ?? '',
          variantLabel: [variantInfo?.color, variantInfo?.size].filter(Boolean).join(' / '),
          quantity,
          unitPrice,
          lineTotal: quantity * unitPrice,
        }
      }),
    }
  })
}

function limitWords(value, maxWords) {
  const words = String(value ?? '').trim().split(/\s+/).filter(Boolean)
  return words.slice(0, maxWords).join(' ')
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
  const { user, login, register, logout, updateUserProfile } = useAuth()
  const [page, setPage] = useState('dashboard')
  const [orders, setOrders] = useState([])
  const [products, setProducts] = useState([])
  const [customers, setCustomers] = useState([])
  const [financialOperations, setFinancialOperations] = useState([])
  const [companyBalance, setCompanyBalance] = useState(null)
  const [categories, setCategories] = useState([])
  const [warehouses, setWarehouses] = useState([])
  const [isLoading, setIsLoading] = useState(false)
  const [apiError, setApiError] = useState('')
  const [operations, setOperations] = useLocalStorage('midas-operations-v2', [])
  const [theme, setTheme] = useLocalStorage('midas-theme', 'dark')
  const [connectedIntegrations, setConnectedIntegrations] = useLocalStorage('midas-connected-integrations', {})
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
      const [
        serverProducts,
        serverVariants,
        serverCategories,
        serverWarehouses,
        serverCustomers,
        serverFinancialOperations,
        serverBalance,
      ] = await Promise.all([
        serverApi.products.getAll(),
        serverApi.productVariants.getAll(),
        serverApi.categories.getAll(),
        serverApi.warehouses.getAll(),
        serverApi.customers.getAll(),
        serverApi.financialOperations.getAll(),
        serverApi.companies.getBalance().catch(() => null),
      ])

      const serverOrders = await serverApi.orders.getAll()
      const nextCustomers = buildCustomerModels(serverCustomers)
      const nextProducts = buildProductModels(serverProducts, serverVariants, serverCategories, serverWarehouses, serverOrders)

      setCategories(serverCategories)
      setWarehouses(serverWarehouses)
      setCustomers(nextCustomers)
      setProducts(nextProducts)
      setOrders(buildOrderModels(serverOrders, nextCustomers, nextProducts))
      setFinancialOperations(serverFinancialOperations)
      setCompanyBalance(serverBalance)
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
        setApiError('Сесія завершилась. Увійдіть ще раз, щоб завантажити компанії та продажі.')
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

  const notifications = useMemo(
    () => {
      const receivedOrders = orders
        .filter((order) => Number(order.status) === 5)
        .slice(0, 5)
        .map((order) => ({
          id: `received-${order.id}`,
          title: `Покупець забрав посилку ${order.code || ''}`.trim(),
          description: `Замовлення на ${Number(order.total).toLocaleString('uk-UA')} грн отримано. Сервер має нарахувати кошти на рахунок компанії.`,
        }))

      return receivedOrders
    },
    [orders],
  )

  const companyOperations = useMemo(
    () => operations.filter((operation) => String(operation.companyId) === String(activeCompanyId)),
    [activeCompanyId, operations],
  )

  const historyOperations = useMemo(
    () => {
      const financialHistory = financialOperations
        .filter((operation) => String(getValue(operation, 'companyId', 'CompanyId')) === String(activeCompanyId))
        .map((operation) => ({
          id: getValue(operation, 'id', 'Id'),
          companyId: getValue(operation, 'companyId', 'CompanyId'),
          createdAt: formatDateTime(new Date(getValue(operation, 'createdAt', 'CreatedAt'))),
          type: Number(getValue(operation, 'operationType', 'OperationType')) === 1 ? 'Поповнення рахунку' : 'Фінансова витрата',
          description: getValue(operation, 'comment', 'Comment') || 'Фінансова операція',
          actor: getValue(operation, 'createdByUserId', 'CreatedByUserId') || 'company',
          amount: `${Number(getValue(operation, 'amount', 'Amount') ?? 0).toLocaleString('uk-UA')} грн`,
        }))

      return [...financialHistory, ...companyOperations]
    },
    [activeCompanyId, companyOperations, financialOperations],
  )

  const stats = useMemo(() => {
    const revenue = orders.reduce((sum, order) => sum + order.total, 0)
    const grossProfit = orders.reduce((sum, order) => sum + order.profit, 0)
    const writeOffs = financialOperations
      .filter((operation) => Number(getValue(operation, 'operationType', 'OperationType')) === 2)
      .reduce((sum, operation) => sum + Number(getValue(operation, 'amount', 'Amount') ?? 0), 0)

    return {
      sales: orders.length,
      customers: customers.length,
      products: products.length,
      expensesCount: financialOperations.length,
      revenue,
      grossProfit,
      expenses: writeOffs,
      loss: writeOffs,
    }
  }, [customers.length, financialOperations, orders, products.length])

  function addOperation(operation) {
    setOperations((currentOperations) => [
      {
        id: crypto.randomUUID(),
        companyId: activeCompanyId,
        createdAt: formatDateTime(new Date()),
        actor: user?.email ?? 'system',
        ...operation,
      },
      ...currentOperations,
    ])
  }

  async function addSale(sale) {
    const deliveryPointType = sale.deliveryPointType === 'parcelLocker' ? 1 : 0
    const selectedCustomer = customers.find((customer) => customer.id === normalizeId(sale.customerId))
    const resolvedCustomer = sale.isNewCustomer
      ? {
        firstName: sale.newCustomer?.name?.trim(),
        surname: sale.newCustomer?.surname?.trim() || '-',
        phone: sale.newCustomer?.phone?.trim(),
        email: sale.newCustomer?.email?.trim(),
      }
      : {
        firstName: selectedCustomer?.firstName || selectedCustomer?.name,
        surname: selectedCustomer?.surname || '-',
        phone: selectedCustomer?.phone,
        email: selectedCustomer?.email,
      }

    if (!resolvedCustomer.firstName || !resolvedCustomer.phone || !resolvedCustomer.email) {
      throw new Error('Заповніть дані клієнта')
    }

    if (!sale.items?.length) {
      throw new Error('Додайте хоча б один товар у замовлення')
    }

    await serverApi.orders.createOneClick({
      customer: {
        name: resolvedCustomer.firstName,
        surname: resolvedCustomer.surname,
        contactValue: resolvedCustomer.phone,
        email: resolvedCustomer.email,
      },
      address: {
        city: sale.city || 'Київ',
        postalCode: Number(sale.postalCode) || 1,
        postDepartmentNumber: Number(sale.postDepartmentNumber) || 1,
        deliveryPointType,
      },
      serviceType: Number(sale.serviceType),
      cargoType: Number(sale.cargoType),
      description: limitWords(sale.description || 'CRM order', 20),
      paymentMethods: Number(sale.paymentMethods),
      items: sale.items.map((item) => ({
        productVariantId: Number(item.productVariantId),
        quantity: Number(item.quantity),
      })),
    })

    await loadServerData()
    setPage('orders')
  }

  async function addFinancialOperation(operation) {
    const createdOperation = await serverApi.financialOperations.create(operation)
    const [nextOperations, nextBalance] = await Promise.all([
      serverApi.financialOperations.getAll(),
      serverApi.companies.getBalance().catch(() => null),
    ])

    setFinancialOperations(nextOperations)
    setCompanyBalance(nextBalance)

    addOperation({
      type: Number(operation.operationType) === 1 ? 'Поповнення рахунку' : 'Фінансова витрата',
      description: operation.comment || 'Фінансова операція',
      amount: `${Number(operation.amount).toLocaleString('uk-UA')} грн`,
    })

    return createdOperation
  }

  async function createCustomer(payload) {
    await serverApi.customers.create(payload)
    await loadServerData()
    setPage('customers')
  }

  async function deleteCustomer(customerId) {
    await serverApi.customers.remove(customerId)
    await loadServerData()
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

  async function sendOrderToNovaPoshta(orderId) {
    const response = await serverApi.novaPoshta.createDocument(orderId)
    await loadServerData()
    return response
  }

  async function connectIntegration(integrationId, token) {
    if (integrationId === 'nova-post') {
      await serverApi.userIntegrations.saveToken('novaposhta', token)
    }

    setConnectedIntegrations((currentIntegrations) => ({
      ...currentIntegrations,
      [integrationId]: {
        connected: true,
        connectedAt: new Date().toISOString(),
        tokenPreview: `${token.slice(0, 4)}...${token.slice(-4)}`,
      },
    }))
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
        operations={historyOperations.slice(0, 5)}
      />
    ),
    orders: <OrdersPage orders={orders} onNavigate={setPage} onSendToNovaPoshta={sendOrderToNovaPoshta} />,
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
    finances: (
      <FinancesPage
        balance={companyBalance ?? activeCompany}
        finances={financialOperations}
        onCreate={addFinancialOperation}
      />
    ),
    customers: <CustomersPage customers={customers} onNavigate={setPage} onDelete={deleteCustomer} />,
    createCustomer: (
      <CreateCustomerPage
        onBack={() => setPage('customers')}
        onCreate={createCustomer}
      />
    ),
    operations: <OperationsPage operations={historyOperations} />,
    profile: <ProfilePage user={user} onUpdateProfile={updateUserProfile} />,
    novaPostIntegration: (
      <NovaPoshtaIntegrationPage
        connection={connectedIntegrations['nova-post']}
        onBack={() => setPage('dashboard')}
        onConnect={(token) => connectIntegration('nova-post', token)}
      />
    ),
    novaPostLogisticProfile: (
      <NovaPoshtaLogisticProfilePage
        onBack={() => setPage('orders')}
        onSaved={() => setPage('orders')}
      />
    ),
    olxIntegration: (
      <OlxIntegrationPage
        connection={connectedIntegrations.olx}
        onBack={() => setPage('dashboard')}
        onConnect={(token) => connectIntegration('olx', token)}
      />
    ),
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
      notifications={notifications}
      connectedIntegrations={connectedIntegrations}
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

