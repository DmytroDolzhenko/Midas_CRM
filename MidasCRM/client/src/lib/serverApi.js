import { apiRequest } from './apiClient.js'

export const serverApi = {
  auth: {
    login(credentials) {
      return apiRequest('/Auth/login', {
        method: 'POST',
        body: {
          email: credentials.email,
          password: credentials.password,
        },
      })
    },
    register(credentials) {
      return apiRequest('/Auth/register', {
        method: 'POST',
        body: {
          email: credentials.email,
          phoneNumber: credentials.phoneNumber,
          password: credentials.password,
          name: credentials.name,
          surname: credentials.surname,
          fathername: credentials.fathername,
        },
      })
    },
  },
  companies: {
    getAll() {
      return apiRequest('/Company')
    },
    getById(id) {
      return apiRequest(`/Company/${id}`)
    },
    getBalance(options = {}) {
      return apiRequest('/Company/balance', options)
    },
    create(company) {
      return apiRequest('/Company', {
        method: 'POST',
        body: company,
      })
    },
    update(id, company) {
      return apiRequest(`/Company/${id}`, {
        method: 'PUT',
        body: company,
      })
    },
    remove(id) {
      return apiRequest(`/Company/${id}`, {
        method: 'DELETE',
      })
    },
    addMemberByEmail(companyId, email) {
      return apiRequest('/Company/add-member', {
        method: 'POST',
        body: { companyId, email },
      })
    },
  },
  companyMembers: {
    getAll() {
      return apiRequest('/CompanyMember')
    },
    updateRole(userId, role) {
      return apiRequest(`/CompanyMember/${userId}/role`, {
        method: 'PUT',
        body: { role },
      })
    },
    remove(userId) {
      return apiRequest(`/CompanyMember/${userId}`, {
        method: 'DELETE',
      })
    },
  },
  products: {
    getAll() {
      return apiRequest('/Product')
    },
    create(product) {
      return apiRequest('/Product', {
        method: 'POST',
        body: product,
      })
    },
    createWithVariants(product) {
      return apiRequest('/Product/product-with-variants', {
        method: 'POST',
        body: product,
      })
    },
    addImage(productId, file) {
      const formData = new FormData()
      formData.append('file', file)

      return apiRequest(`/Product/${productId}/images`, {
        method: 'POST',
        body: formData,
      })
    },
    setMainImage(productId, imageId) {
      return apiRequest(`/Product/${productId}/images/${imageId}/main`, {
        method: 'PATCH',
      })
    },
  },
  productVariants: {
    getAll() {
      return apiRequest('/ProductVariant')
    },
    create(variant) {
      return apiRequest('/ProductVariant', {
        method: 'POST',
        body: variant,
      })
    },
  },
  orders: {
    getAll() {
      return apiRequest('/Order')
    },
    update(orderId, order) {
      return apiRequest(`/Order/${orderId}`, {
        method: 'PUT',
        body: order,
      })
    },
    updateStatus(orderId, status) {
      return apiRequest('/Order/update-status', {
        method: 'PATCH',
        body: { orderId, status },
      })
    },
    createOneClick(order) {
      return apiRequest('/Order/one-click', {
        method: 'POST',
        body: order,
      })
    },
  },
  novaPoshta: {
    createDocument(orderId) {
      return apiRequest(`/NovaPoshta/documents/${orderId}`, {
        method: 'POST',
      })
    },
    syncDirectories() {
      return apiRequest('/NovaPoshta/sync-directories', {
        method: 'POST',
      })
    },
    getSenders() {
      return apiRequest('/novaposhta/settings/senders')
    },
    saveLogisticProfile(payload) {
      return apiRequest('/novaposhta/settings/logistic-profile', {
        method: 'POST',
        body: payload,
      })
    },
  },
  userIntegrations: {
    saveToken(provider, token) {
      return apiRequest('/UserIntegration/save-token', {
        method: 'POST',
        body: { provider, token },
      })
    },
  },
  ai: {
    generateDescription(payload) {
      return apiRequest('/ai-agent/generate-description', {
        method: 'POST',
        body: payload,
      })
    },
  },
  customers: {
    getAll() {
      return apiRequest('/Customer')
    },
    create(customer) {
      return apiRequest('/Customer', {
        method: 'POST',
        body: customer,
      })
    },
    remove(id) {
      return apiRequest(`/Customer/${id}`, {
        method: 'DELETE',
      })
    },
  },
  financialOperations: {
    getAll() {
      return apiRequest('/FinancialOperation')
    },
    create(operation) {
      return apiRequest('/FinancialOperation', {
        method: 'POST',
        body: operation,
      })
    },
    remove(id) {
      return apiRequest(`/FinancialOperation/${id}`, {
        method: 'DELETE',
      })
    },
  },
  categories: {
    getAll() {
      return apiRequest('/ProductCategory')
    },
  },
  warehouses: {
    getAll() {
      return apiRequest('/Warehouse')
    },
    create(warehouse) {
      return apiRequest('/Warehouse', {
        method: 'POST',
        body: warehouse,
      })
    },
    update(id, warehouse) {
      return apiRequest(`/Warehouse/${id}`, {
        method: 'PUT',
        body: warehouse,
      })
    },
  },
}
