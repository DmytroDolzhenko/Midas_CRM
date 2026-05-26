# Midas CRM Client

Simple React + Vite client without TypeScript. Components use `.jsx`, plain
modules use `.js`.

## Scripts

```bash
npm run dev
npm run build
npm run lint
```

On Windows PowerShell, use `npm.cmd` if script execution is blocked:

```bash
npm.cmd run dev
```

## Structure

```text
src/
  components/       shared UI elements
  features/         feature-specific CRM logic
    auth/           login feature
      api/
      components/
      hooks/
      types.js
    customers/      customer feature UI
    sales/          orders, products and sales logic
  hooks/            global hooks
  lib/              api client and mock data
  pages/            screens that compose features
  App.jsx           app state and navigation
  main.jsx          React entry point
  styles.css        global styles
```

## Current Features

- Login screen with demo credentials.
- Dashboard with totals from current CRM state.
- Orders list with search.
- Product list with search.
- Customers list.
- Create order screen.
- Create product screen.
- Orders, products, customers and user session are stored in `localStorage`.

When the backend endpoints are ready, replace mock data in `src/lib/mockData.js`
with requests through `src/lib/apiClient.js`.
