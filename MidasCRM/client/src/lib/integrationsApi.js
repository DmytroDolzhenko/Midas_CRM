export const integrations = [
  {
    id: 'nova-post',
    pageId: 'novaPostIntegration',
    name: 'Нова Пошта',
    description: 'ТТН, відстеження посилок, статуси отримання та автоматичні події для замовлень.',
    status: 'Потрібен API token',
    accent: 'Доставка',
    enabled: false,
  },
  {
    id: 'olx',
    pageId: 'olxIntegration',
    name: 'OLX',
    description: 'Підготовка до синхронізації оголошень, заявок покупців і майбутнього імпорту замовлень.',
    status: 'Потрібен access token',
    accent: 'Маркетплейс',
    enabled: false,
  },
]

export async function connectIntegration(id) {
  return { id, status: 'connected' }
}

export async function disconnectIntegration(id) {
  return { id, status: 'disconnected' }
}

export async function generateNovaPostTtn(payload) {
  return {
    ttn: `NP${Date.now()}`,
    payload,
    status: 'created',
  }
}

export async function syncMarketplaceOrders(source) {
  return {
    source,
    syncedAt: new Date().toISOString(),
    status: 'queued',
  }
}
