export const integrations = [
  {
    id: 'nova-post',
    name: 'Нова Пошта',
    description: 'Генерація ТТН, відстеження статусів доставки та оновлення продажів.',
    enabled: true,
  },
  {
    id: 'olx',
    name: 'OLX',
    description: 'Синхронізація замовлень, залишків і повідомлень покупців.',
    enabled: false,
  },
  {
    id: 'telegram',
    name: 'Telegram',
    description: 'Прийом діалогів клієнтів і швидкі відповіді з CRM.',
    enabled: false,
  },
  {
    id: 'instagram',
    name: 'Instagram',
    description: 'Директ-повідомлення, заявки та комунікація з покупцями.',
    enabled: true,
  },
  {
    id: 'prom',
    name: 'Prom.ua',
    description: 'Синхронізація продажів, товарів, залишків і статусів.',
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
