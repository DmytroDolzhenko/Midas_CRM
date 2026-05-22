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
    createOneClick(order) {
      return apiRequest('/Order/one-click', {
        method: 'POST',
        body: order,
      })
    },
  },
  customers: {
    getAll() {
      return apiRequest('/Customer')
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
  },
}
